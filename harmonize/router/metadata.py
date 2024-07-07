import datetime
from pathlib import Path
from typing import Literal, cast
from fastapi import APIRouter
from fastapi.responses import FileResponse
from mutagen.easyid3 import EasyID3
from mutagen.mp3 import MP3
from harmonize.const import MUSIC_ROOT, MUSIC_ROOT_LEGACY, TMP_ALBUM_ART_DIR
from harmonize.defs.metadata import MediaMetadata

router = APIRouter()


@router.get("/metadata/media/{filename}")
def media_metadata(filename: str) -> MediaMetadata:
    track = MP3(MUSIC_ROOT / filename)
    tags = EasyID3(MUSIC_ROOT / filename)

    pict = cast(bytes, track.get("APIC:").data)  # type: ignore

    # Make album art dir in case it doesn't exist yet
    TMP_ALBUM_ART_DIR.mkdir(parents=True, exist_ok=True)
    temp_albumart_name = f"{filename}_{
        datetime.datetime.now().timestamp()}.png"
    Path(TMP_ALBUM_ART_DIR / temp_albumart_name).write_bytes(pict)

    src_url = f"{TMP_ALBUM_ART_DIR}/{temp_albumart_name}"

    return {
        "title": _get_str_tag(tags, "title"),
        "album": _get_str_tag(tags, "album"),
        "artist": _get_str_tag(tags, "artist"),
        "artwork": [
            {
                "src": src_url,
                "name": temp_albumart_name,
                "sizes": "1200x1200",
                "type": "image/png"
            }
        ],
    }


def _get_str_tag(tags: EasyID3, tag: Literal["title", "album", "artist"]) -> str:
    return cast(list[str], tags.get(tag))[0]


@router.get("/metadata/albumart/{filename}")
def album_art(filename: str) -> FileResponse:
    return FileResponse(TMP_ALBUM_ART_DIR / filename)
