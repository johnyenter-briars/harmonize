import datetime
import json
import logging
from pathlib import Path

import yt_dlp
from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session

from harmonize.const import YOUTUBE_SEARCH_METADATA
from harmonize.db.database import get_session
from harmonize.db.models import Job, Status
from harmonize.defs.response import BaseResponse
from harmonize.job.methods import start_job

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api')

_audio_format = 'mp3'


@router.post('/download/youtube/{id}', status_code=201)
async def download_youtube(
    id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    args: tuple[str] = (id,)

    metadata_file = YOUTUBE_SEARCH_METADATA / f'{id}.search.info.json'

    if not Path.exists(metadata_file):
        raise HTTPException(status_code=400, detail='Youtube metadata not present')

    with metadata_file.open('r') as f:
        metadata = json.loads(f.read())
        title = metadata['title']
        description = f'download youtube video: {title}'

    job = Job(
        description=description,
        status=Status.RUNNING,
        started_on=datetime.datetime.now(datetime.UTC),
    )
    session.add(job)
    session.commit()

    job = await start_job(_download_youtube, job, session, args)

    return {'message': 'Job created', 'status_code': 201, 'value': job}


def _download_youtube(
    id: str,
    job: Job,
    session: Session,
):
    try:
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
                job.status = Status.FAILED
                return
        job.status = Status.SUCCEEDED
    except Exception:
        job.status = Status.FAILED
    finally:
        session.add(job)
        session.commit()
