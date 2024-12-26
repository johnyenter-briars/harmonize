import logging
import uuid

from fastapi import APIRouter, Depends
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, MediaEntryType
from harmonize.defs.response import BaseResponse

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/media')


@router.get('/video', status_code=200)
async def list_video(
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where(MediaEntry.type == MediaEntryType.VIDEO)
    media_entries = session.exec(statement).all()
    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=list(media_entries)
    )


@router.get('/audio', status_code=200)
async def list_audio(
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where(MediaEntry.type == MediaEntryType.AUDIO)
    media_entries = session.exec(statement).all()
    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=list(media_entries)
    )


@router.delete('/{media_entry_id}', status_code=200)
async def delete_media_entry(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    statement = select(MediaEntry).where(MediaEntry.id == media_entry_id)

    media_entry = session.exec(statement).first()

    if not media_entry:
        return BaseResponse[None](message='Media entry not found', status_code=404, value=None)

    session.delete(media_entry)
    session.commit()

    return BaseResponse[None](message='Entry deleted', status_code=204, value=None)
