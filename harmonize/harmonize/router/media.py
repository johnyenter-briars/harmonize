import logging
import uuid

from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy import select
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, MediaEntryType
from harmonize.defs.media import UpsertMediaEntryRequest
from harmonize.defs.response import BaseResponse

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/media')


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
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where(MediaEntry.type == MediaEntryType.VIDEO)

    if name_sub_string:
        statement = statement.where(MediaEntry.name.like(f'%{name_sub_string}%'))  # type: ignore

    statement = statement.offset(skip).limit(limit)
    media_entries = list(session.exec(statement).all())

    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=media_entries
    )


@router.get('/video/{media_entry_id}/sub', status_code=200)
async def get_video_sub(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[MediaEntry]:
    entry = session.get(MediaEntry, media_entry_id)

    if entry is None:
        return BaseResponse[MediaEntry](message='Entry not found', status_code=404, value=None)

    subtitle_id = entry.subtitle_file_id

    if subtitle_id is None:
        return BaseResponse[MediaEntry](message='Subtitle tot found', status_code=404, value=None)

    subtitle = session.get(MediaEntry, subtitle_id)

    return BaseResponse[MediaEntry](message='Media Entries Found', status_code=200, value=subtitle)


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

    session.delete(media_entry)
    session.commit()

    return BaseResponse[None](message='Entry deleted', status_code=200, value=None)
