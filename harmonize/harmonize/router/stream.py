from collections.abc import Generator
from pathlib import Path
from typing import Any

from fastapi import APIRouter
from fastapi.responses import StreamingResponse

from harmonize.const import MUSIC_ROOT

router = APIRouter()


def _stream_file(path: Path) -> Generator[bytes, Any, None]:
    with path.open('rb') as file_bytes:
        yield from file_bytes


@router.get('/stream/{filename}')
async def stream_file(filename: str) -> StreamingResponse:
    return StreamingResponse(
        _stream_file(MUSIC_ROOT / Path(filename)),
        media_type='audio/mp3',
    )
