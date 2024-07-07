from fastapi import APIRouter
from fastapi.responses import FileResponse
import yt_dlp
from harmonize.const import MUSIC_ROOT_LEGACY

router = APIRouter()


# TODO: I doubt this should return a `dict` - probably like a 201 or something?
@router.post("/download/youtube/{id}")
def download_youtube(id: str) -> dict:
    url = f'https://www.youtube.com/watch?v={id}'
    ydl_opts = {
        "outtmpl": "./media/video/%(title)s.%(ext)s"
    }
    # TODO: store metadata?
    # with yt_dlp.YoutubeDL(ydl_opts) as ydl:
    #     info = ydl.extract_info(url, download=False)
    #     output = json.dumps(ydl.sanitize_info(info))
    #     with open("./media/video/output.info.json", "w") as f:
    #         f.write(output)
    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])

    ydl_audo_opts = {
        'format': 'm4a/bestaudio/best',
        "outtmpl": "./media/audio/%(title)s.%(ext)s",
        # ℹ️ See help(yt_dlp.postprocessor) for a list of available Postprocessors and their arguments
        'postprocessors': [{  # Extract audio using ffmpeg
            'key': 'FFmpegExtractAudio',
            'preferredcodec': 'm4a',
        }]
    }

    with yt_dlp.YoutubeDL(ydl_audo_opts) as ydl:
        error_code = ydl.download([url])
    return {}


@router.get("/download/{filename}")
def download(filename: str) -> FileResponse:
    return FileResponse(MUSIC_ROOT_LEGACY / filename)
