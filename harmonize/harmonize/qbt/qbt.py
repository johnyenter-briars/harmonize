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
    remove_file,
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


def _create_media_entry(
    file: Path,
    moved_path: Path,
    download: QbtDownloadData,
    tag_data: QbtDownloadTagInfo,
    season_id: uuid.UUID | None,
    entry_id: uuid.UUID | None = None,
    subtitle_id: uuid.UUID | None = None,
) -> MediaEntry:
    media_entry = MediaEntry(
        name=file.name,
        absolute_path=moved_path.absolute().as_posix(),
        source=MediaElementSource.MAGNETLINK,
        youtube_id=None,
        magnet_link=download.magnet_uri,
        type=MediaEntryType.VIDEO
        if file.suffix.lower() in VIDEO_EXTENSIONS
        else MediaEntryType.SUBTITLE,
        video_type=tag_data.video_type,
        audio_type=tag_data.audio_type,
        date_added=datetime.datetime.now(datetime.UTC),
        cover_art_absolute_path=None,
        thumbnail_art_absolute_path=None,
        season_id=season_id,
        subtitle_file_id=None,
    )

    if entry_id is not None:
        media_entry.id = entry_id

    if subtitle_id is not None:
        media_entry.subtitle_file_id = subtitle_id

    return media_entry


def _save_file(
    source_path: Path, download: QbtDownloadData, session: Session, logger: logging.Logger
):
    statement = select(QbtDownloadTagInfo).where(
        QbtDownloadTagInfo.magnet_link.ilike(download.magnet_uri)  # type: ignore
    )

    tag_data = next(iter(session.exec(statement)))

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
        subtitle_file_id=None,
    )

    session.add(media_entry)
    session.delete(tag_data)
    session.commit()

    logger.debug('Added media entry: %s', media_entry.id)

    remove_file(source_path)


def _list_files_recursive(directory: str) -> list[Path]:
    return [file for file in Path(directory).rglob('*') if file.is_file()]


def _process_files(
    eligible_files: list[Path],
    tag_data: QbtDownloadTagInfo,
    download: QbtDownloadData,
    session: Session,
    logger: logging.Logger,
) -> tuple[bool, str | None]:
    chosen_drive = get_drive_with_least_space()

    season_id = None
    if tag_data.create_season:
        season_id = uuid.uuid4()
        season = Season(name=download.name, id=season_id)
        session.add(season)

        logger.info('Created season: %s', season_id)

    srt_matching: dict[str, list[Path]] = {}
    for file in eligible_files:
        if file.stem in srt_matching:
            srt_matching[file.stem].append(file)
        else:
            srt_matching[file.stem] = [file]

    for name in srt_matching:
        files = srt_matching.get(name)
        if files is None:
            continue

        if len(files) == 1:
            file = files[0]

            moved_path = move_file_to_mounted_folders(file.absolute(), chosen_drive)
            if moved_path is None:
                return (False, f'Unable to move file: {file.absolute()}')

            session.add(_create_media_entry(file, moved_path, download, tag_data, season_id))
        else:
            srt_files = [
                file
                for file in files
                if file.suffix == '.srt' or file.suffix is None or file.suffix == ''
            ]

            if len(srt_files) != 1:
                names = ', '.join([file.name for file in srt_files])
                return (False, f'More than one srt file associated! {names}')

            srt_file = srt_files[0]
            srt_id = uuid.uuid4()

            moved_path = move_file_to_mounted_folders(srt_file.absolute(), chosen_drive)
            if moved_path is None:
                return (False, f'Unable to move file: {srt_file.absolute()}')

            srt_media_entry = _create_media_entry(
                srt_file, moved_path, download, tag_data, season_id, entry_id=srt_id
            )

            session.add(srt_media_entry)

            video_files = [file for file in files if file.suffix in VIDEO_EXTENSIONS]

            if len(video_files) != 1:
                names = ', '.join([file.name for file in video_files])
                return (False, f'More than one video file associated! {names}')

            video_file = video_files[0]

            moved_path = move_file_to_mounted_folders(video_file.absolute(), chosen_drive)
            if moved_path is None:
                return (False, f'Unable to move file: {video_file.absolute()}')

            video_media_entry = _create_media_entry(
                video_file, moved_path, download, tag_data, season_id, subtitle_id=srt_id
            )

            session.add(video_media_entry)

    return (True, None)


def _save_directory_files(
    source_path: Path, download: QbtDownloadData, session: Session, logger: logging.Logger
) -> bool:
    if not source_path.is_dir():
        logger.error('Provided path is not a directory: %s', source_path)
        return False

    statement = select(QbtDownloadTagInfo).where(
        QbtDownloadTagInfo.magnet_link.ilike(download.magnet_uri)  # type: ignore
    )

    tag_data = next(iter(session.exec(statement)))

    all_files_recursive = _list_files_recursive(source_path.absolute().as_posix())

    eligible_files = [
        file
        for file in all_files_recursive
        if file.is_file()
        and (
            file.suffix.lower() in SUPPORTED_EXTENSIONS
            or file.suffix.lower() == ''
            or file.suffix is None
        )
    ]

    (success, error_message) = _process_files(eligible_files, tag_data, download, session, logger)

    logger.info('Success %s', success)
    logger.info('Error message %s', error_message)

    if success:
        session.delete(tag_data)
        session.commit()
        logger.info('Processed all files in directory: %s', source_path)
        return True

    logger.error('Failure to process all files in directory: %s', source_path)
    return False


async def qbt_background_service():
    while True:
        try:
            session: Session = get_session_non_gen()
            downloads = await list_downloads()
            for download in downloads:
                if _download_finished(download) and not _media_entry_exists(download, session):
                    source_path = Path(download.content_path)
                    if source_path.is_dir():
                        should_delete_download = _save_directory_files(
                            source_path, download, session, logger
                        )
                    else:
                        should_delete_download = _save_file(source_path, download, session, logger)

                    # if should_delete_download:
                    #     remove_file(source_path)
                    #     await delete_download(download.hash)

            logger.info('%s is running...', 'qbt_background_service')
            await asyncio.sleep(30)
        except Exception as e:
            logger.exception('Error in background service: %s', str(e))
        finally:
            if session:  # type: ignore
                session.close()
