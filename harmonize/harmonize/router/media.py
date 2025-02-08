import logging
import uuid
from operator import iand
from pathlib import Path

from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy import select
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, MediaEntryType, VideoType
from harmonize.defs.media import UpsertMediaEntryRequest
from harmonize.defs.response import BaseResponse
from harmonize.file.drive import remove_file

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/media')


@router.get('/entry/{media_entry_id}', status_code=200)
async def get_entry(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[MediaEntry]:
    entry = session.get(MediaEntry, media_entry_id)

    if entry is None:
        return BaseResponse[MediaEntry](message='Entry not found', status_code=404, value=None)

    return BaseResponse[MediaEntry](message='Media Entry Found', status_code=200, value=entry)


@router.get('/audio', status_code=200)
async def list_audio(
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where(MediaEntry.type == MediaEntryType.AUDIO)
    media_entries = session.exec(statement).all()
    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=list(media_entries)
    )


@router.get('/video', status_code=200)
async def list_video_paging(
    limit: int = Query(10, ge=1),
    skip: int = Query(0, ge=0),
    name_sub_string: str | None = Query(None),
    type: list[VideoType] = Query(None),
    transferred: bool | None = Query(None),
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where(MediaEntry.type == MediaEntryType.VIDEO)

    if name_sub_string:
        statement = statement.where(MediaEntry.name.like(f'%{name_sub_string}%'))  # type: ignore

    if type:
        statement = statement.where(MediaEntry.video_type.in_(type))  # type: ignore

    if transferred is not None and transferred is True:
        statement = statement.where(MediaEntry.transferred)

    statement = statement.offset(skip).limit(limit)

    statement = statement.order_by(MediaEntry.date_added.desc())  # type: ignore

    media_entries = list(session.exec(statement).all())

    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=media_entries
    )


@router.get('/video/{media_entry_id}/sub', status_code=200)
async def get_sub(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    entry = session.get(MediaEntry, media_entry_id)

    if entry is None:
        return BaseResponse[list[MediaEntry]](
            message='Entry not found', status_code=404, value=None
        )

    statement = select(MediaEntry).where(
        iand(
            MediaEntry.type == MediaEntryType.SUBTITLE, MediaEntry.parent_media_entry_id == entry.id
        )
    )

    subs = list(session.exec(statement))

    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=subs
    )


@router.get('/sub', status_code=200)
async def list_sub_paging(
    limit: int = Query(10, ge=1),
    skip: int = Query(0, ge=0),
    name_sub_string: str | None = Query(None),
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where(MediaEntry.type == MediaEntryType.SUBTITLE)

    if name_sub_string:
        statement = statement.where(MediaEntry.name.like(f'%{name_sub_string}%'))  # type: ignore

    statement = statement.offset(skip).limit(limit)
    media_entries = list(session.exec(statement).all())

    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=media_entries
    )


@router.put('/{media_entry_id}', status_code=200)
async def update_media_entry(
    media_entry_id: uuid.UUID,
    req: UpsertMediaEntryRequest,
    session: Session = Depends(get_session),
) -> BaseResponse[MediaEntry]:
    media_entry = session.get(MediaEntry, media_entry_id)

    if not media_entry:
        raise HTTPException(status_code=404, detail='Media entry not found')

    media_entry.name = req.name
    session.commit()
    session.refresh(media_entry)

    return BaseResponse[MediaEntry](message='Entry updated', status_code=200, value=media_entry)


@router.delete('/{media_entry_id}', status_code=200)
async def delete_media_entry(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    media_entry = session.get(MediaEntry, media_entry_id)

    if not media_entry:
        raise HTTPException(status_code=404, detail='Media entry not found')

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

    session.commit()

    return BaseResponse[None](message='Entry deleted', status_code=200, value=None)
