from pathlib import Path
from typing import Any, Generator
from fastapi import APIRouter
from fastapi.responses import StreamingResponse

from ..const import MUSIC_ROOT

router = APIRouter()


def _stream_file(path: Path) -> Generator[bytes, Any, None]:
    with path.open("rb") as file_bytes:
        yield from file_bytes


@router.get("/stream/{filename}")
def stream_file(filename: str) -> StreamingResponse:
    foo = MUSIC_ROOT / Path(filename)
    return StreamingResponse(
        _stream_file(MUSIC_ROOT / Path(filename)), media_type="audio/mp3"
    )
