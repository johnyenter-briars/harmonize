import uuid
from collections.abc import Generator
from pathlib import Path
from typing import Any

from fastapi import APIRouter, Depends, HTTPException
from fastapi.responses import StreamingResponse
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry

router = APIRouter(prefix='/api')


def _stream_file(path: Path) -> Generator[bytes, Any, None]:
    with path.open('rb') as file_bytes:
        yield from file_bytes


@router.get('/stream/{id}')
async def stream_file(
    id: uuid.UUID,
    session: Session = Depends(get_session),
) -> StreamingResponse:
    statement = select(MediaEntry).where(MediaEntry.id == id)
    media_entries = list(session.exec(statement).all())

    if len(media_entries) > 1:
        raise HTTPException(
            status_code=500, detail=f'More than one media entry found with id: {id}'
        )

    if len(media_entries) == 0:
        raise HTTPException(status_code=400, detail=f'No media entry found with id: {id}')

    full_path = media_entries[0].absolute_path

    return StreamingResponse(
        _stream_file(Path(full_path)),
        media_type='audio/mp3',
    )
