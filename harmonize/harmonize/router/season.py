import logging
import uuid

from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, Season
from harmonize.defs.response import BaseResponse

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/season')


@router.post('/{season_name}', status_code=201)
async def create_season(
    season_name: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    season = Season(name=season_name)
    session.add(season)
    session.commit()
    session.refresh(season)
    return BaseResponse[Season](
        message='Season created successfully', status_code=201, value=season
    )


@router.post('/{season_id}/associate', status_code=200)
async def associate_media_entries(
    season_id: uuid.UUID,
    media_entry_ids: list[uuid.UUID],
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    season = session.get(Season, season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    for media_entry_id in media_entry_ids:
        media_entry = session.get(MediaEntry, media_entry_id)
        if media_entry:
            media_entry.season_id = season_id
        else:
            logger.warning(f'Media entry with ID {media_entry_id} not found.')

    session.commit()
    return BaseResponse[None](
        message='Media entries associated successfully', status_code=200, value=None
    )


@router.post('/{season_id}/disassociate', status_code=200)
async def disassociate_media_entries(
    season_id: uuid.UUID,
    media_entry_ids: list[uuid.UUID],
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    # for media_entry_id in media_entry_ids:
    #     media_entry = session.get(MediaEntry, media_entry_id)
    #     if media_entry and media_entry.season_id == season_id:
    #         media_entry.season_id = None
    #     elif media_entry:
    #         logger.warning(
    #             f'Media entry {media_entry_id} is not associated with season {season_id}.'
    #         )

    session.commit()
    return BaseResponse[None](
        message='Media entries disassociated successfully', status_code=200, value=None
    )


@router.get('/{season_id}', status_code=200)
async def get_season_details(
    season_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[Season]:
    season = session.get(Season, season_id)
    if not season:
        raise HTTPException(status_code=404, detail='Season not found')

    # Fetch the season along with its associated media entries
    season_with_media = session.exec(select(Season).where(Season.id == season_id)).first()

    return BaseResponse[Season](
        message='Season details retrieved successfully', status_code=200, value=season_with_media
    )


@router.get('/', status_code=200)
async def get_seasons(
    session: Session = Depends(get_session),
) -> BaseResponse[list[Season]]:
    seasons = list(session.exec(select(Season)).all())

    return BaseResponse[list[Season]](
        message='Season details retrieved successfully', status_code=200, value=seasons
    )
