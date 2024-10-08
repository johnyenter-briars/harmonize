import datetime
import json
import logging
from pathlib import Path
from typing import Any

import yt_dlp
from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select

from harmonize.const import (
    MUSIC_ROOT,
    YOUTUBE_PLAYLIST_SEARCH_METADATA,
    YOUTUBE_VIDEO_SEARCH_METADATA,
)
from harmonize.db.database import get_session
from harmonize.db.models import Job, JobStatus, MediaElementSource, MediaElementType, MediaEntry
from harmonize.defs.response import BaseResponse
from harmonize.job.methods import start_job

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api')

_audio_format = 'mp3'


@router.post('/download/youtube/video/{id}', status_code=201)
async def download_youtube_video(
    id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    statement = select(MediaEntry).where(MediaEntry.youtube_id == id)
    media_entries = session.exec(statement).all()

    if len(media_entries) > 0:
        raise HTTPException(status_code=400, detail='Media entry already exists')

    metadata_file = YOUTUBE_VIDEO_SEARCH_METADATA / f'{id}.search.info.json'

    if not Path.exists(metadata_file):
        raise HTTPException(status_code=400, detail='Youtube metadata not present')

    with metadata_file.open('r') as f:
        metadata = json.loads(f.read())
        title = metadata['title']
        description = f'download youtube video: {title}'

    args: tuple[str, Any] = (id, metadata)

    job = Job(
        description=description,
        status=JobStatus.RUNNING,
        started_on=datetime.datetime.now(datetime.UTC),
        error_message=None,
    )
    session.add(job)
    session.commit()

    job = await start_job(_download_youtube_video, job, session, args)

    return {'message': 'Job created', 'status_code': 201, 'value': job}


def _download_youtube_video(
    id: str,
    yt_metadata: Any,
    job: Job,
    session: Session,
):
    try:
        yt_title = yt_metadata['title']

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
            'outtmpl': (MUSIC_ROOT / f'{yt_title}').absolute().as_posix(),
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
                job.status = JobStatus.FAILED
                job.error_message = f'Youtube Download Failed with error code {error_code}'

        job.status = JobStatus.SUCCEEDED

        media_element = MediaEntry(
            name=yt_title,
            relative_path=(MUSIC_ROOT / f'{yt_title}.mp3').absolute().as_posix(),
            source=MediaElementSource.YOUTUBE,
            youtube_id=id,
            type=MediaElementType.MUSIC,
            date_added=datetime.datetime.now(datetime.UTC),
        )

        session.add(media_element)

    except Exception as e:
        job.status = JobStatus.FAILED
        job.error_message = str(e)
    finally:
        session.add(job)
        session.commit()


@router.post('/download/youtube/playlist/{id}', status_code=201)
async def download_youtube_playlist(
    id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    args: tuple[str] = (id,)

    metadata_file = YOUTUBE_PLAYLIST_SEARCH_METADATA / f'{id}.search.info.json'

    if not Path.exists(metadata_file):
        raise HTTPException(status_code=400, detail='Youtube metadata not present')

    with metadata_file.open('r') as f:
        metadata = json.loads(f.read())
        title = metadata['title']
        description = f'download youtube playlist: {title}'

    job = Job(
        description=description,
        status=JobStatus.RUNNING,
        started_on=datetime.datetime.now(datetime.UTC),
        error_message=None,
    )
    session.add(job)
    session.commit()

    # job = await start_job(_download_youtube_playlist, job, session, args)

    # return {'message': 'Job created', 'status_code': 201, 'value': job}
    return {'message': 'Job created', 'status_code': 201, 'value': None}


def _download_youtube_playlist(
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

        ydl_audio_opts = {
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

        with yt_dlp.YoutubeDL(ydl_audio_opts) as ydl:
            error_code = ydl.download([url])
            if error_code:
                logger.error(
                    'Youtube Download Failed with error code', extra={'error_code': error_code}
                )
                job.status = JobStatus.FAILED
                return
        job.status = JobStatus.SUCCEEDED
    except Exception as e:
        job.status = JobStatus.FAILED
        job.error_message = str(e)
    finally:
        session.add(job)
        session.commit()
