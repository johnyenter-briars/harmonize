import json
import os
from typing import Tuple
from fastapi import APIRouter
from fastapi.responses import FileResponse
import yt_dlp
from harmonize.const import MUSIC_ROOT_LEGACY
from harmonize.defs.media import MediaDownload, MediaElement
from harmonize.defs.job import Job, Status
from harmonize.job import start_job

router = APIRouter()

_audio_format = "m4a"

# TODO: I doubt this should return a `dict` - probably like a 201 or something?
@router.post("/download/youtube/{id}")
async def download_youtube(id: str) -> Job:
    args: Tuple[str] = (id,)
    job_info = await start_job(_download_youtube, args)

    return job_info


def _download_youtube(id: str, job_info: Job):
    url = f'https://www.youtube.com/watch?v={id}'
    ydl_opts = {
        "outtmpl": "./media/video/%(title)s.%(ext)s"
    }

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        info = ydl.extract_info(url, download=False)
        output = json.dumps(ydl.sanitize_info(info))
        with open(f"./cache/youtube/metadata/{id}.info.json", "w") as f:
            f.write(output)

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])


    ydl_audo_opts = {
        'format': f'{_audio_format}/bestaudio/best',
        "outtmpl": "./media/audio/%(title)s.%(ext)s",
        # ℹ️ See help(yt_dlp.postprocessor) for a list of available Postprocessors and their arguments
        'postprocessors': [{  # Extract audio using ffmpeg
            'key': 'FFmpegExtractAudio',
            'preferredcodec': _audio_format,
        }]
    }

    with yt_dlp.YoutubeDL(ydl_audo_opts) as ydl:
        error_code = ydl.download([url])
    

    job_info["status"] = Status.SUCCEEDED



@router.get("/download/{filename}")
def download(filename: str) -> FileResponse:
    return FileResponse(MUSIC_ROOT_LEGACY / filename)
