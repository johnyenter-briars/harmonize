import datetime
import json
import logging
from enum import Enum
from pathlib import Path
from typing import Any

import eyed3
import yt_dlp
from eyed3.id3.frames import ImageFrame
from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select

from harmonize.const import (
    AUDIO_ROOT,
    TMP_ALBUM_ART_DIR,
    VIDEO_ROOT,
    YOUTUBE_PLAYLIST_SEARCH_METADATA,
    YOUTUBE_PLAYLIST_YTDL_METADATA,
    YOUTUBE_VIDEO_SEARCH_METADATA,
    YOUTUBE_VIDEO_YTDL_METADATA,
)
from harmonize.db.database import get_session
from harmonize.db.models import Job, JobStatus, MediaElementSource, MediaEntry, MediaEntryType
from harmonize.defs.response import BaseResponse
from harmonize.defs.youtube import DownloadPlaylistArguments, DownloadVideoArguments
from harmonize.harmonize.job.callback import start_job
from harmonize.util.filepropety import convert_webp_to_jpeg, rescale_jpeg
from harmonize.util.metadata import (
    download_image,
)

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/youtube')

_audio_format = 'mp3'


class YoutubeElementType(Enum):
    VIDEO = 0
    PLAYLIST = 1


def _save_image(
    youtube_id: str, youtube_title: str, youtube_metadata: Any, object_type: YoutubeElementType
) -> Path | None:
    temp_path_to_image_webp = TMP_ALBUM_ART_DIR / f'{youtube_id}.webp'
    temp_path_to_image_jpeg = TMP_ALBUM_ART_DIR / f'{youtube_id}.jpeg'

    if object_type == YoutubeElementType.VIDEO:
        highest_quality_image = max(
            youtube_metadata['thumbnails'],
            key=lambda thumbnail: thumbnail.get('width', 0) * thumbnail.get('height', 0),
        )
    else:
        highest_quality_image = max(
            youtube_metadata['thumbnails'],
            key=lambda thumbnail: thumbnail.get('width', 0) * thumbnail.get('height', 0),
        )

    url = highest_quality_image['url']

    _ = download_image(url, temp_path_to_image_webp)

    convert_webp_to_jpeg(temp_path_to_image_webp, temp_path_to_image_jpeg)

    rescale_jpeg(temp_path_to_image_jpeg, temp_path_to_image_jpeg)

    return temp_path_to_image_jpeg


def _inject_album_art(path_to_file: Path, path_to_image: Path):
    audiofile = eyed3.load(path_to_file)
    if audiofile is None:
        # TODO
        raise Exception('foo')

    if audiofile is None:
        raise Exception('Audio file could not be loaded.')

    if audiofile.tag is None:
        audiofile.initTag()

    audiofile.tag.images.set(  # type: ignore
        ImageFrame.FRONT_COVER, open(path_to_image, 'rb').read(), 'image/jpeg'
    )  # type: ignore

    audiofile.tag.save()  # type: ignore


@router.post('/video/{video_id}', status_code=201)
async def download_youtube_video(
    video_id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    statement = select(MediaEntry).where(MediaEntry.youtube_id == video_id)
    media_entries = session.exec(statement).all()

    if len(media_entries) > 0:
        raise HTTPException(status_code=400, detail='Media entry already exists')

    metadata_file = YOUTUBE_VIDEO_SEARCH_METADATA / f'{video_id}.search.info.json'

    if not Path.exists(metadata_file):
        raise HTTPException(status_code=400, detail='Youtube metadata not present')

    with metadata_file.open('r') as f:
        metadata = json.loads(f.read())
        title = metadata['title']
        description = f'download youtube video: {title}'

    args: tuple[DownloadVideoArguments] = (
        DownloadVideoArguments(video_id=video_id, video_metadata=metadata),
    )

    job = await start_job(description, _download_youtube_video, session, args)

    return BaseResponse[Job](message='Job created', status_code=201, value=job)


def _download_youtube_video(
    download_video_arguments: DownloadVideoArguments,
    job: Job,
    session: Session,
):
    video_id = download_video_arguments.video_id
    yt_metadata = download_video_arguments.video_metadata

    try:
        yt_title = yt_metadata['title']

        album_art_path = _save_image(video_id, yt_title, yt_metadata, YoutubeElementType.VIDEO)

        url = f'https://www.youtube.com/watch?v={video_id}'
        ydl_opts = {
            'outtmpl': './media/video/%(title)s.%(ext)s',
        }

        with yt_dlp.YoutubeDL(ydl_opts) as ydl:
            info = ydl.extract_info(url, download=False)
            output = json.dumps(ydl.sanitize_info(info))
            (YOUTUBE_VIDEO_YTDL_METADATA / f'{video_id}.info.json').write_text(output)

        ydl_audo_opts = {
            'format': f'{_audio_format}/bestaudio/best',
            'outtmpl': (AUDIO_ROOT / f'{yt_title}').absolute().as_posix(),
            # See help(yt_dlp.postprocessor) for a list of available Postprocessors and their arguments
            'postprocessors': [
                {  # Extract audio using ffmpeg
                    'key': 'FFmpegExtractAudio',
                    'preferredcodec': _audio_format,
                },
            ],
            'postprocessor_args': [
                '-write_id3v1',
                '1',
                '-id3v2_version',
                '3',
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
                return

        absolute_path = AUDIO_ROOT / f'{yt_title}.mp3'

        if album_art_path is not None:
            _inject_album_art(absolute_path, album_art_path)

        if album_art_path is not None:
            _inject_album_art(absolute_path, album_art_path)

        job.status = JobStatus.SUCCEEDED

        media_entry = MediaEntry(
            name=yt_title,
            absolute_path=absolute_path.absolute().as_posix(),
            source=MediaElementSource.YOUTUBE,
            youtube_id=video_id,
            magnet_link=None,
            type=MediaEntryType.AUDIO,
            date_added=datetime.datetime.now(datetime.UTC),
        )

        session.add(media_entry)

        logger.debug('Added media entry: %s', media_entry.id)

    except Exception as e:
        job.status = JobStatus.FAILED
        job.error_message = str(e)
    finally:
        session.add(job)
        session.commit()


@router.post('/playlist/{playlist_id}', status_code=201)
async def download_youtube_playlist(
    playlist_id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    metadata_file = YOUTUBE_PLAYLIST_SEARCH_METADATA / f'{playlist_id}.search.info.json'

    if not Path.exists(metadata_file):
        raise HTTPException(status_code=400, detail='Youtube metadata not present')

    with metadata_file.open('r') as f:
        metadata = json.loads(f.read())
        title = metadata['title']
        description = f'download youtube playlist: {title}'

    args: tuple[DownloadPlaylistArguments] = (
        DownloadPlaylistArguments(playlist_id=playlist_id, playlist_metadata=metadata),
    )

    job = await start_job(description, _download_youtube_playlist, session, args)

    return BaseResponse[Job](message='Job created', status_code=201, value=job)


def _download_youtube_playlist(
    download_playlist_arguments: DownloadPlaylistArguments,
    job: Job,
    session: Session,
):
    playlist_id = download_playlist_arguments.playlist_id
    yt_metadata = download_playlist_arguments.playlist_metadata
    try:
        album_art_path = _save_image(
            playlist_id,
            download_playlist_arguments.playlist_metadata['title'],
            yt_metadata,
            YoutubeElementType.PLAYLIST,
        )
        url = f'https://www.youtube.com/playlist?list={playlist_id}'
        ydl_opts = {
            'outtmpl': (VIDEO_ROOT / '%(playlist)s/%(title)s.%(ext)s').absolute().as_posix(),
        }

        with yt_dlp.YoutubeDL(ydl_opts) as ydl:
            playlist_info = ydl.extract_info(url, download=False)
            output = json.dumps(playlist_info)
            (YOUTUBE_PLAYLIST_YTDL_METADATA / f'{playlist_id}.info.json').write_text(output)

        if playlist_info is None:
            raise Exception('Playlist info malformed')

        video_list = []
        for entry in playlist_info['entries']:
            video_title = entry.get('title')
            video_id = entry.get('id')
            video_list.append({'title': video_title, 'id': video_id})

        ydl_audio_opts = {
            'format': f'{_audio_format}/bestaudio/best',
            'outtmpl': (AUDIO_ROOT / '%(title)s.%(ext)s').absolute().as_posix(),
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
                    'YouTube playlist download failed with error code',
                    extra={'error_code': error_code},
                )
                job.status = JobStatus.FAILED
                return

        for video in video_list:
            absolute_path = AUDIO_ROOT / f"{video['title']}.mp3"

            if album_art_path is not None:
                _inject_album_art(absolute_path, album_art_path)

            media_entry = MediaEntry(
                name=video['title'],
                absolute_path=absolute_path.absolute().as_posix(),
                source=MediaElementSource.YOUTUBE,
                youtube_id=video['id'],
                magnet_link=None,
                type=MediaEntryType.AUDIO,
                date_added=datetime.datetime.now(datetime.UTC),
            )
            session.add(media_entry)

            logger.debug('Added media entry: %s', media_entry.id)

        job.status = JobStatus.SUCCEEDED
    except Exception as e:
        job.status = JobStatus.FAILED
        job.error_message = str(e)
    finally:
        session.add(job)
        session.commit()
