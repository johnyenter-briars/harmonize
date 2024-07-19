import json
import logging
from pathlib import Path

import yt_dlp
from fastapi import APIRouter
from fastapi.responses import FileResponse

from harmonize.const import MUSIC_ROOT_LEGACY
from harmonize.defs.job import Job, Status
from harmonize.job import start_job

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api')

_audio_format = 'm4a'


@router.post('/download/youtube/{id}', status_code=201)
async def download_youtube(id: str) -> Job:
    args: tuple[str] = (id,)
    return await start_job(_download_youtube, args)


def _download_youtube(id: str, job_info: Job):
    url = f'https://www.youtube.com/watch?v={id}'
    ydl_opts = {
        'outtmpl': './media/video/%(title)s.%(ext)s',
    }

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        info = ydl.extract_info(url, download=False)
        output = json.dumps(ydl.sanitize_info(info))
        Path(f'./cache/youtube/metadata/{id}.info.json').write_text(output)

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])

    ydl_audo_opts = {
        'format': f'{_audio_format}/bestaudio/best',
        'outtmpl': './media/audio/%(title)s.%(ext)s',
        # See help(yt_dlp.postprocessor) for a list of available Postprocessors and their arguments
        'postprocessors': [
            {  # Extract audio using ffmpeg
                'key': 'FFmpegExtractAudio',
                'preferredcodec': _audio_format,
            }
        ],
    }

    with yt_dlp.YoutubeDL(ydl_audo_opts) as ydl:
        error_code = ydl.download([url])
        if error_code:
            logger.error(
                'Youtube Download Failed with error code', extra={'error_code': error_code}
            )

    job_info['status'] = Status.SUCCEEDED


@router.get('/download/{filename}')
def download(filename: str) -> FileResponse:
    return FileResponse(MUSIC_ROOT_LEGACY / filename)
