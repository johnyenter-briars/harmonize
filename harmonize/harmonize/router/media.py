import logging

from fastapi import APIRouter, Depends
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry
from harmonize.defs.response import BaseResponse

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/media')


@router.get('/video', status_code=200)
async def list_video(
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where()
    media_entries = session.exec(statement).all()
    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=list(media_entries)
    )


@router.get('/audio', status_code=200)
async def list_audio(
    session: Session = Depends(get_session),
) -> BaseResponse[list[MediaEntry]]:
    statement = select(MediaEntry).where()
    media_entries = session.exec(statement).all()
    return BaseResponse[list[MediaEntry]](
        message='Media Entries Found', status_code=200, value=list(media_entries)
    )
