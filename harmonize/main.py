from pathlib import Path
from fastapi import FastAPI

from harmonize.router import stream


app = FastAPI()

app.include_router(stream.router)

# MUSIC_ROOT_LEGACY = Path("./music")
# MEDIA_ROOT = Path("./media/audio/youtube")
# MUSIC_ROOT = Path("./media/audio")


@app.get("/")
async def root():
    return {"message": "Hello from harmonize!"}


# @app.get("/search/youtube/{search_keywords}")
# def search_youtube(search_keywords: str) -> dict:
#     videosSearch = VideosSearch(search_keywords, limit=10)

#     search_result: dict = videosSearch.result()  # type: ignore

#     with open(f"./cache/{search_keywords}.info.json", "w") as f:
#         f.write(json.dumps(search_result))

#     return search_result


# # TODO: I doubt this should return a `dict` - probably like a 201 or something?
# @app.post("/download/youtube/{id}")
# def download_youtube(id: str) -> dict:
#     url = f'https://www.youtube.com/watch?v={id}'
#     ydl_opts = {
#         "outtmpl": "./media/video/%(title)s.%(ext)s"
#     }
#     # TODO: store metadata?
#     # with yt_dlp.YoutubeDL(ydl_opts) as ydl:
#     #     info = ydl.extract_info(url, download=False)
#     #     output = json.dumps(ydl.sanitize_info(info))
#     #     with open("./media/video/output.info.json", "w") as f:
#     #         f.write(output)
#     with yt_dlp.YoutubeDL(ydl_opts) as ydl:
#         ydl.download([url])
#     return {}


# @app.get("/media")
# def list_media() -> dict[str, list[str]]:
#     albums: dict[str, list[str]] = {}
#     for item in MEDIA_ROOT.iterdir():
#         print(item)
#         if item.is_dir():
#             albums[item.name] = [song.name for song in item.iterdir()]
#     return albums


# @app.get("/list_music")
# def list_music() -> dict[str, list[str]]:
#     albums: dict[str, list[str]] = {}
#     for item in MUSIC_ROOT_LEGACY.iterdir():
#         if item.is_dir():
#             albums[item.name] = [song.name for song in item.iterdir()]
#     return albums


# @app.get("/download/{filename}")
# def download(filename: str) -> FileResponse:
#     return FileResponse(MUSIC_ROOT_LEGACY / filename)


# def get_str_tag(tags: EasyID3, tag: Literal["title", "album", "artist"]) -> str:
#     return cast(list[str], tags.get(tag))[0]


# @app.get("/media_metadata/{filename}")
# def media_metadata(filename: str) -> metadata.MediaMetadata:
#     track = MP3(MUSIC_ROOT_LEGACY / filename)
#     tags = EasyID3(MUSIC_ROOT_LEGACY / filename)

#     pict = cast(bytes, track.get("APIC:").data)  # type: ignore

#     # Make album art dir in case it doesn't exist yet
#     TMP_ALBUM_ART_DIR.mkdir(parents=True, exist_ok=True)
#     temp_albumart_name = f"{filename}_{
#         datetime.datetime.now().timestamp()}.png"
#     Path(TMP_ALBUM_ART_DIR / temp_albumart_name).write_bytes(pict)

#     src_url = f"album_art/{temp_albumart_name}"

#     return {
#         "title": get_str_tag(tags, "title"),
#         "album": get_str_tag(tags, "album"),
#         "artist": get_str_tag(tags, "artist"),
#         "artwork": [{"src": src_url, "sizes": "1200x1200", "type": "image/png"}],
#     }


# @app.get("/album_art/{filename}")
# def album_art(filename: str) -> FileResponse:
#     return FileResponse(TMP_ALBUM_ART_DIR / filename)
