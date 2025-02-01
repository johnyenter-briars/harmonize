import logging
import uuid

from fastapi import APIRouter, Depends, HTTPException, Query
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
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = (
        select(MediaEntry).where(MediaEntry.type == MediaEntryType.VIDEO).offset(skip).limit(limit)
    )
    media_entries = session.exec(statement).all()
    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=list(media_entries)
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
