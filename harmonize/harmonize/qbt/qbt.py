import asyncio
import datetime
import logging
import uuid
from codecs import encode
from pathlib import Path

import aiohttp
import pydantic
from sqlalchemy.orm import Session
from sqlmodel import Session, select

from harmonize.config.harmonizeconfig import HARMONIZE_CONFIG
from harmonize.const import SUPPORTED_EXTENSIONS, VIDEO_EXTENSIONS
from harmonize.db.database import get_session_non_gen
from harmonize.db.models import (
    MediaElementSource,
    MediaEntry,
    MediaEntryType,
    QbtDownloadTagInfo,
    Season,
)
from harmonize.defs.qbt import QbtDownloadData
from harmonize.file.drive import (
    get_drive_with_least_space,
    move_file_to_mounted_folders,
)

logger = logging.getLogger('harmonize')

_adapter = pydantic.TypeAdapter(list[QbtDownloadData])


async def list_downloads() -> list[QbtDownloadData]:
    qbt_domain_name = HARMONIZE_CONFIG.qbt_domain_name
    qbt_port = HARMONIZE_CONFIG.qbt_port
    qbt_api_version = HARMONIZE_CONFIG.qbt_version
    url = f'http://{qbt_domain_name}:{qbt_port}/api/{qbt_api_version}/torrents/info'
    async with aiohttp.ClientSession() as session, session.get(url) as resp:
        raw_json = await resp.json()
        data = _adapter.validate_python(raw_json)
        return data


async def add_torrent(magnet_link: str) -> str:
    qbt_domain_name = HARMONIZE_CONFIG.qbt_domain_name
    qbt_port = HARMONIZE_CONFIG.qbt_port
    qbt_api_version = HARMONIZE_CONFIG.qbt_version
    url = f'http://{qbt_domain_name}:{qbt_port}/api/{qbt_api_version}/torrents/add'
    headers = {'Content-type': 'multipart/form-data; boundary=wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'}

    dataList = []
    boundary = 'wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'
    dataList.append(encode('--' + boundary))
    dataList.append(encode('Content-Disposition: form-data; name=urls;'))
    dataList.append(encode('Content-Type: {}'.format('text/plain')))
    dataList.append(encode(''))
    dataList.append(encode(magnet_link))
    dataList.append(encode('--' + boundary + '--'))
    dataList.append(encode(''))
    body = b'\r\n'.join(dataList)
    payload = body

    async with (
        aiohttp.ClientSession() as session,
        session.post(url, headers=headers, data=payload) as response,
    ):
        response_text = await response.text()
        return response_text


async def pause_torrent(torrent_hash: str) -> str:
    qbt_domain_name = HARMONIZE_CONFIG.qbt_domain_name
    qbt_port = HARMONIZE_CONFIG.qbt_port
    qbt_api_version = HARMONIZE_CONFIG.qbt_version
    url = f'http://{qbt_domain_name}:{qbt_port}/api/{qbt_api_version}/torrents/stop'
    headers = {'Content-type': 'multipart/form-data; boundary=wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'}

    dataList = []
    boundary = 'wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'
    dataList.append(encode('--' + boundary))
    dataList.append(encode('Content-Disposition: form-data; name=hashes;'))
    dataList.append(encode('Content-Type: {}'.format('text/plain')))
    dataList.append(encode(''))
    dataList.append(encode(torrent_hash))
    dataList.append(encode('--' + boundary + '--'))
    dataList.append(encode(''))
    body = b'\r\n'.join(dataList)
    payload = body

    async with (
        aiohttp.ClientSession() as session,
        session.post(url, headers=headers, data=payload) as response,
    ):
        response_text = await response.text()
        return response_text


async def resume_download(torrent_hash: str) -> str:
    qbt_domain_name = HARMONIZE_CONFIG.qbt_domain_name
    qbt_port = HARMONIZE_CONFIG.qbt_port
    qbt_api_version = HARMONIZE_CONFIG.qbt_version
    url = f'http://{qbt_domain_name}:{qbt_port}/api/{qbt_api_version}/torrents/start'
    headers = {'Content-type': 'multipart/form-data; boundary=wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'}

    dataList = []
    boundary = 'wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'
    dataList.append(encode('--' + boundary))
    dataList.append(encode('Content-Disposition: form-data; name=hashes;'))
    dataList.append(encode('Content-Type: {}'.format('text/plain')))
    dataList.append(encode(''))
    dataList.append(encode(torrent_hash))
    dataList.append(encode('--' + boundary + '--'))
    dataList.append(encode(''))
    body = b'\r\n'.join(dataList)
    payload = body

    async with (
        aiohttp.ClientSession() as session,
        session.post(url, headers=headers, data=payload) as response,
    ):
        response_text = await response.text()
        return response_text


async def delete_download(torrent_hash: str) -> str:
    qbt_domain_name = HARMONIZE_CONFIG.qbt_domain_name
    qbt_port = HARMONIZE_CONFIG.qbt_port
    qbt_api_version = HARMONIZE_CONFIG.qbt_version
    url = f'http://{qbt_domain_name}:{qbt_port}/api/{qbt_api_version}/torrents/delete'
    boundary = 'wL36Yn8afVp8Ag7AmP8qZ0SA4n1v9T'
    headers = {'Content-type': f'multipart/form-data; boundary={boundary}'}
    dataList = []
    dataList.append('--' + boundary)
    dataList.append('Content-Disposition: form-data; name=hashes;')
    dataList.append('Content-Type: {}'.format('text/plain'))
    dataList.append('')
    dataList.append(torrent_hash)

    dataList.append('--' + boundary)
    dataList.append('Content-Disposition: form-data; name=deleteFiles;')
    dataList.append('Content-Type: {}'.format('text/plain'))
    dataList.append('')
    dataList.append('false')

    dataList.append('--' + boundary + '--')
    dataList.append('')

    body = '\r\n'.join(dataList)

    async with (
        aiohttp.ClientSession() as session,
        session.post(url, data=body, headers=headers) as resp,
    ):
        response_text = await resp.text()
        return response_text


def _download_finished(data: QbtDownloadData) -> bool:
    return data.state in ('stalledUP', 'uploading', 'stoppedUP')


def _media_entry_exists(
    data: QbtDownloadData,
    session: Session,
) -> bool:
    statement = select(MediaEntry).where(MediaEntry.magnet_link == data.magnet_uri)
    media_entries = list(session.exec(statement).all())
    return len(media_entries) > 0


def save_file(download: QbtDownloadData, session: Session, logger: logging.Logger):
    statement = select(QbtDownloadTagInfo).where(
        QbtDownloadTagInfo.magnet_link.ilike(download.magnet_uri)  # type: ignore
    )

    tag_data = next(iter(session.exec(statement)))

    source_path = Path(download.content_path)

    moved_path = move_file_to_mounted_folders(source_path)
    if moved_path is None:
        msg = 'Unable to move file'
        raise Exception(msg)  # noqa: TRY002

    media_entry = MediaEntry(
        name=download.name,
        absolute_path=moved_path.absolute().as_posix(),
        source=MediaElementSource.MAGNETLINK,
        youtube_id=None,
        magnet_link=download.magnet_uri,
        type=tag_data.type,
        video_type=tag_data.video_type,
        audio_type=tag_data.audio_type,
        date_added=datetime.datetime.now(datetime.UTC),
        cover_art_absolute_path=None,
        thumbnail_art_absolute_path=None,
        season_id=None,
    )

    session.add(media_entry)
    session.delete(tag_data)
    session.commit()

    logger.debug('Added media entry: %s', media_entry.id)

    # remove_file(source_path)


def save_directory_files(download: QbtDownloadData, session: Session, logger: logging.Logger):
    path = Path(download.content_path)

    if not path.is_dir():
        logger.error('Provided path is not a directory: %s', path)
        return

    season_id = uuid.uuid4()
    season = Season(name=download.name, id=season_id)
    session.add(season)

    logger.info('Created season: %s', season_id)

    chosen_drive = get_drive_with_least_space()

    foo = path.iterdir()

    for file in path.iterdir():
        if file.is_file() and file.suffix.lower() in SUPPORTED_EXTENSIONS:
            source_path = file.absolute()
            moved_path = move_file_to_mounted_folders(source_path, chosen_drive)
            if moved_path is None:
                msg = 'Unable to move file'
                raise Exception(msg)  # noqa: TRY002

            media_entry = MediaEntry(
                name=file.stem,
                absolute_path=moved_path.absolute().as_posix(),
                source=MediaElementSource.MAGNETLINK,
                youtube_id=None,
                magnet_link=download.magnet_uri,
                type=MediaEntryType.VIDEO
                if file.suffix.lower() in VIDEO_EXTENSIONS
                else MediaEntryType.SUBTITLE,
                video_type=None,
                audio_type=None,
                date_added=datetime.datetime.now(datetime.UTC),
                cover_art_absolute_path=None,
                thumbnail_art_absolute_path=None,
                season_id=season_id,
            )

            session.add(media_entry)

            logger.debug('Added media entry: %s', media_entry.id)

    session.commit()
    logger.info('Processed all files in directory: %s', path)
    # remove_file(path)


async def qbt_background_service():
    while True:
        try:
            session: Session = get_session_non_gen()
            downloads = await list_downloads()
            for download in downloads:
                if _download_finished(download) and not _media_entry_exists(download, session):
                    path = Path(download.content_path)
                    # if path.is_dir():
                    #     save_directory_files(download, session, logger)
                    # else:
                    #     save_file(download, session, logger)
                    save_file(download, session, logger)

                    # await delete_download(download.hash)

            logger.info('%s is running...', 'qbt_background_service')
            await asyncio.sleep(30)
        except Exception as e:
            logger.exception('Error in background service: %s', str(e))
        finally:
            if session:  # type: ignore
                session.close()
