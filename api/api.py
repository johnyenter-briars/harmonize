import datetime
from typing import Any, Generator, Literal, cast
from fastapi import FastAPI
from pathlib import Path

from fastapi.responses import FileResponse, StreamingResponse

from mutagen.easyid3 import EasyID3

from mutagen.mp3 import MP3

from defs import MediaMetadata

app = FastAPI()

MUSIC_ROOT = Path("./music")

TMP_ALBUM_ART_DIR = Path("/tmp/album_art")


def stream_file(path: Path) -> Generator[bytes, Any, None]:
    with path.open("rb") as file_bytes:
        yield from file_bytes


@app.get("/")
def root() -> Literal["Hello world"]:
    return "Hello world"


@app.get("/list_music")
def list_music() -> dict[str, list[str]]:
    albums: dict[str, list[str]] = {}
    for item in MUSIC_ROOT.iterdir():
        if item.is_dir():
            albums[item.name] = [song.name for song in item.iterdir()]
    return albums


@app.get("/stream/{filename}")
def stream(filename: str) -> StreamingResponse:
    return StreamingResponse(
        stream_file(MUSIC_ROOT / Path(filename)), media_type="audio/mp3"
    )


@app.get("/download/{filename}")
def download(filename: str) -> FileResponse:
    return FileResponse(MUSIC_ROOT / filename)


def get_str_tag(tags: EasyID3, tag: Literal["title", "album", "artist"]) -> str:
    return cast(list[str], tags.get(tag))[0]


@app.get("/media_metadata/{filename}")
def media_metadata(filename: str) -> MediaMetadata:
    track = MP3(MUSIC_ROOT / filename)
    tags = EasyID3(MUSIC_ROOT / filename)

    pict = cast(bytes, track.get("APIC:").data)  # type: ignore

    # Make album art dir in case it doesn't exist yet
    TMP_ALBUM_ART_DIR.mkdir(parents=True, exist_ok=True)
    temp_albumart_name = f"{filename}_{datetime.datetime.now().timestamp()}.png"
    Path(TMP_ALBUM_ART_DIR / temp_albumart_name).write_bytes(pict)

    src_url = f"album_art/{temp_albumart_name}"

    return {
        "title": get_str_tag(tags, "title"),
        "album": get_str_tag(tags, "album"),
        "artist": get_str_tag(tags, "artist"),
        "artwork": [{"src": src_url, "sizes": "1200x1200", "type": "image/png"}],
    }


@app.get("/album_art/{filename}")
def album_art(filename: str) -> FileResponse:
    return FileResponse(TMP_ALBUM_ART_DIR / filename)
