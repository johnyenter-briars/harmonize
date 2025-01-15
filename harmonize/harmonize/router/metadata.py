import logging
import uuid
from pathlib import Path

from fastapi import APIRouter, Depends, HTTPException
from fastapi.responses import FileResponse
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry

logger = logging.getLogger('harmonize')

router = APIRouter(prefix='/api/metadata')


@router.get('/audio/thumbnail/{id}')
async def thumbnail_art(id: uuid.UUID, session: Session = Depends(get_session)) -> FileResponse:
    statement = select(MediaEntry).where(MediaEntry.id == id)
    media_entry = session.exec(statement).first()
    if media_entry is None:
        raise HTTPException(status_code=400, detail='Media entry not found')

    if media_entry.thumbnail_art_absolute_path is None:
        raise HTTPException(
            status_code=400, detail='Thumbnail art path not available for this media entry'
        )

    return FileResponse(Path(media_entry.thumbnail_art_absolute_path))


@router.get('/audio/cover/{id}')
async def cover_art(id: uuid.UUID, session: Session = Depends(get_session)) -> FileResponse:
    statement = select(MediaEntry).where(MediaEntry.id == id)
    media_entry = session.exec(statement).first()
    if media_entry is None:
        raise HTTPException(status_code=400, detail='Media entry not found')

    if media_entry.cover_art_absolute_path is None:
        raise HTTPException(
            status_code=400, detail='Cover art path not available for this media entry'
        )

    return FileResponse(Path(media_entry.cover_art_absolute_path))
