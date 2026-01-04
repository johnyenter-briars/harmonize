import logging
import uuid
from operator import iand
from pathlib import Path

from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy import and_, select
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, MediaEntryType, Season, VideoType
from harmonize.defs.response import BaseResponse
from harmonize.defs.season import (
    AssociateToSeasonRequest,
    DisassociateToSeasonRequest,
    UpsertSeasonRequest,
)
from harmonize.file.drive import remove_file

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/season')


@router.post('/', status_code=201)
async def create_season(
    req: UpsertSeasonRequest,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    season = Season(name=req.name)
    session.add(season)
    session.commit()
    session.refresh(season)
    return BaseResponse[Season](
        message='Season created successfully', status_code=201, value=season
    )


@router.post('/associate', status_code=200)
async def associate_media_entries(
    req: AssociateToSeasonRequest,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    season = session.get(Season, req.season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    for media_entry_id in req.media_entry_ids:
        media_entry = session.get(MediaEntry, media_entry_id)
        if media_entry:
            media_entry.season_id = req.season_id
        else:
            logger.warning(f'Media entry with ID {media_entry_id} not found.')

    session.commit()
    return BaseResponse[Season](
        message='Media entries associated successfully', status_code=200, value=None
    )


@router.post('/disassociate', status_code=200)
async def disassociate_media_entries(
    req: DisassociateToSeasonRequest,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    for media_entry_id in req.media_entry_ids:
        media_entry = session.get(MediaEntry, media_entry_id)
        if media_entry and media_entry.season_id == req.season_id:
            media_entry.season_id = None
        elif media_entry:
            logger.warning(
                f'Media entry {media_entry_id} is not associated with season {req.season_id}.'
            )

    session.commit()
    return BaseResponse[Season](
        message='Media entries disassociated successfully', status_code=200, value=None
    )


@router.get('/entries/{season_id}', status_code=200)
async def get_season_entries(
    season_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    season = session.get(Season, season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    query = select(MediaEntry).where(
        and_(
            MediaEntry.season_id == season_id,  # type: ignore
            MediaEntry.video_type == VideoType.EPISODE,  # type: ignore
            MediaEntry.type == MediaEntryType.VIDEO,  # type: ignore
        )
    )

    media_entries = list(session.exec(query))

    return BaseResponse[list[MediaEntry]](
        message='Season details retrieved successfully', status_code=200, value=media_entries
    )


@router.get('/{season_id}', status_code=200)
async def get_season(
    season_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    season = session.get(Season, season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    return BaseResponse[Season](
        message='Season retrieved successfully', status_code=200, value=season
    )


@router.get('', status_code=200)
async def get_seasons(
    limit: int = Query(10, ge=1),
    skip: int = Query(0, ge=0),
    name_sub_string: str | None = Query(None),
    session: Session = Depends(get_session),
) -> BaseResponse[list[Season]]:
    statement = select(Season)

    if name_sub_string:
        statement = statement.where(Season.name.like(f'%{name_sub_string}%'))  # type: ignore

    statement = statement.offset(skip).limit(limit)

    statement = statement.order_by(Season.date_added.desc())  # type: ignore

    seasons = list(session.exec(statement).all())

    return BaseResponse[list[Season]](
        message='Season details retrieved successfully', status_code=200, value=seasons
    )


@router.put('/{season_id}', status_code=200)
async def update_season(
    season_id: uuid.UUID,
    req: UpsertSeasonRequest,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    season = session.get(Season, season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    season.name = req.name
    session.commit()
    session.refresh(season)

    return BaseResponse[Season](
        message='Season updated successfully', status_code=200, value=season
    )


@router.delete('/{season_id}', status_code=200)
async def delete_season(
    season_id: uuid.UUID,
    delete_episodes: bool | None = Query(None),
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    season = session.get(Season, season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    if delete_episodes is not None and delete_episodes:
        media_entries = list(
            session.exec(
                select(MediaEntry).where(
                    MediaEntry.season_id == season_id,
                )
            )
        )

        for media_entry in media_entries:
            subs = list(
                session.exec(
                    select(MediaEntry).where(
                        iand(
                            MediaEntry.type == MediaEntryType.SUBTITLE,
                            MediaEntry.parent_media_entry_id == media_entry.id,
                        )
                    )
                )
            )

            for sub in subs:
                remove_file(Path(sub.absolute_path))
                session.delete(sub)

            remove_file(Path(media_entry.absolute_path))
            session.delete(media_entry)

    session.refresh(season)
    session.delete(season)
    session.commit()

    return BaseResponse[None](message='Season deleted successfully', status_code=200, value=None)
