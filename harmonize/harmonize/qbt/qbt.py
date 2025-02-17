import asyncio
import datetime
import logging
import uuid
from codecs import encode
from operator import iand
from pathlib import Path
from urllib.parse import parse_qs, unquote, urlparse

import aiohttp
import pydantic
import requests
from sqlalchemy.orm import Session
from sqlmodel import Session, select

from harmonize.config.harmonizeconfig import HARMONIZE_CONFIG
from harmonize.const import SUBTITLE_EXTENSIONS, SUPPORTED_EXTENSIONS, VIDEO_EXTENSIONS
from harmonize.db.database import get_new_session, get_session_non_gen
from harmonize.db.models import (
    Job,
    JobStatus,
    MediaElementSource,
    MediaEntry,
    MediaEntryType,
    QbtDownloadTagInfo,
    Season,
)
from harmonize.defs.qbt import QbtDownloadData
from harmonize.file.drive import (
    copy_file_to_mounted_folders,
    get_drive_with_most_space,
    list_files_recursive,
)
from harmonize.job.callback import start_job

logger = logging.getLogger('harmonize')

_adapter = pydantic.TypeAdapter(list[QbtDownloadData])


def normalize_magnet_link(link: str) -> str:
    """Normalize magnet links for consistent comparison."""
    parsed = urlparse(link.lower())  # Ensure case insensitivity
    params = parse_qs(unquote(parsed.query))  # Decode and parse query params
    base = parsed.scheme + '?' + parsed.netloc + parsed.path  # Normalize base URL
    sorted_params = '&'.join(
        f'{k}={",".join(sorted(v))}' for k, v in sorted(params.items())
    )  # Sort params
    return base + '?' + sorted_params


def compare_magnet_links(link1: str, link2: str) -> bool:
    """Compare two magnet links after normalization."""
    return normalize_magnet_link(link1) == normalize_magnet_link(link2)


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


def delete_download_sync(torrent_hash: str, delete_files: bool = False) -> str:
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
    if delete_files:
        dataList.append('true')
    else:
        dataList.append('false')

    dataList.append('--' + boundary + '--')
    dataList.append('')

    body = '\r\n'.join(dataList)

    response = requests.post(url, data=body, headers=headers)
    return response.text


def _download_finished(data: QbtDownloadData) -> bool:
    return data.state in ('stalledUP', 'uploading', 'stoppedUP')


def _media_entry_exists(
    data: QbtDownloadData,
    session: Session,
) -> bool:
    statement = select(MediaEntry).where(MediaEntry.magnet_link == data.magnet_uri)
    media_entries = list(session.exec(statement).all())
    return len(media_entries) > 0


def _job_exists(
    job_key: str,
    session: Session,
) -> bool:
    jobs = list(
        session.exec(select(Job).where(iand(Job.key == job_key, Job.status == JobStatus.RUNNING)))
    )
    return len(jobs) > 0


def _create_media_entry(
    file: Path,
    moved_path: Path,
    download: QbtDownloadData,
    tag_data: QbtDownloadTagInfo,
    season_id: uuid.UUID | None,
    entry_id: uuid.UUID | None = None,
    parent_id: uuid.UUID | None = None,
    name: str | None = None,
) -> MediaEntry:
    media_entry = MediaEntry(
        name=file.stem,
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
        transferred=False,
        season_id=season_id,
        parent_media_entry_id=None,
    )

    if entry_id is not None:
        media_entry.id = entry_id

    if parent_id is not None:
        media_entry.parent_media_entry_id = parent_id

    if name is not None:
        media_entry.name = name

    return media_entry


def _save_file(
    source_path: Path, download: QbtDownloadData, session: Session, logger: logging.Logger
):
    results = list(session.exec(select(QbtDownloadTagInfo)))

    tag_data = next(
        (r for r in results if compare_magnet_links(str(r.magnet_link), str(download.magnet_uri))),
        None,
    )

    if tag_data is None:
        logger.info('No tag data for currently running download: %s', download.name)
        return False

    moved_path = copy_file_to_mounted_folders(source_path)
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
        transferred=False,
        season_id=None,
        parent_media_entry_id=None,
    )

    session.add(media_entry)
    session.delete(tag_data)
    session.commit()

    logger.debug('Added media entry: %s', media_entry.id)

    return True


def _subtitle_match(potential_subtitle_file: Path, video_file: Path):
    video_stem = video_file.stem

    """
    folder/foobar.mkv
    folder/subs/foobar.srt
    """
    if potential_subtitle_file.stem == video_stem:
        return True

    """
    folder/foobar.mkv
    folder/english.srt
    or
    folder/foobar.mkv
    folder/foobar.srt
    """
    if potential_subtitle_file.parent == video_file.parent:
        return True

    """
    folder/foobar.mkv
    folder/foobar/english.srt
    """
    if video_stem.lower() in str(potential_subtitle_file).lower():  # noqa: SIM103
        return True

    return False


def _process_files(
    eligible_files: list[Path],
    tag_data: QbtDownloadTagInfo,
    download: QbtDownloadData,
    session: Session,
    logger: logging.Logger,
) -> tuple[bool, str | None]:
    chosen_drive = get_drive_with_most_space()

    logger.info('Chose drive: %s', chosen_drive)

    season_id = None
    if tag_data.create_season:
        season_id = uuid.uuid4()
        season = Season(name=download.name, id=season_id)
        session.add(season)
        session.commit()

        logger.info('Created season: %s', season_id)

    # Match up srt files with their respective video files
    srt_matching: dict[str, list[Path]] = {}
    for file in [file for file in eligible_files if file.suffix in VIDEO_EXTENSIONS]:
        srt_matching[file.stem] = [file]

    for potential_subtitle_file in [
        file for file in eligible_files if file.suffix in SUBTITLE_EXTENSIONS
    ]:
        for key, current_files in srt_matching.items():
            video_files = [file for file in current_files if file.suffix in VIDEO_EXTENSIONS]

            if len(video_files) > 1:
                return (
                    False,
                    f'Error in matching up srt files. More than one video in value list for: {key}',
                )

            video_file = video_files[0]

            if _subtitle_match(potential_subtitle_file, video_file):
                srt_matching[key].append(potential_subtitle_file)

    # Process each pair of video + zero or many srt files
    for name in srt_matching:
        tag_data = session.merge(tag_data)
        session.refresh(tag_data)

        files = srt_matching.get(name)
        if files is None:
            continue

        if len(files) == 1:
            file = files[0]

            moved_path = copy_file_to_mounted_folders(file.absolute(), chosen_drive)
            if moved_path is None:
                return (False, f'Unable to move file: {file.absolute()}')

            session.add(_create_media_entry(file, moved_path, download, tag_data, season_id))
            session.commit()
        else:
            # Process video first
            video_files = [file for file in files if file.suffix in VIDEO_EXTENSIONS]

            if len(video_files) != 1:
                names = ', '.join([file.name for file in video_files])
                return (False, f'More than one video file associated! {names}')

            video_file = video_files[0]
            video_file_id = uuid.uuid4()

            moved_path = copy_file_to_mounted_folders(video_file.absolute(), chosen_drive)
            if moved_path is None:
                return (False, f'Unable to move file: {video_file.absolute()}')

            video_media_entry = _create_media_entry(
                video_file, moved_path, download, tag_data, season_id, entry_id=video_file_id
            )

            session.add(video_media_entry)
            session.commit()

            logger.info(
                'Added entry: %s, %s, %s',
                video_media_entry.name,
                video_media_entry.id,
                video_media_entry.type,
            )

            # Process related srts
            for srt_file in [file for file in files if file.suffix in SUBTITLE_EXTENSIONS]:
                srt_name = f'{video_file.stem}-{srt_file.stem}'

                moved_path = copy_file_to_mounted_folders(
                    srt_file.absolute(), chosen_drive, srt_name
                )

                if moved_path is None:
                    return (False, f'Unable to move file: {srt_file.absolute()}')

                srt_media_entry = _create_media_entry(
                    srt_file,
                    moved_path,
                    download,
                    tag_data,
                    season_id,
                    parent_id=video_file_id,
                    name=srt_name,
                )

                session.add(srt_media_entry)
                session.commit()

                logger.info(
                    'Added entry: %s, %s, %s',
                    srt_media_entry.name,
                    srt_media_entry.id,
                    srt_media_entry.type,
                )

    session.delete(tag_data)
    session.commit()

    return (True, None)


def _save_directory_files(
    source_path: Path, download: QbtDownloadData, session: Session, logger: logging.Logger
) -> bool:
    logger.info('Beginning to process: %s', download.name)

    if not source_path.is_dir():
        logger.error('Provided path is not a directory: %s', source_path)
        return False

    results = list(session.exec(select(QbtDownloadTagInfo)))

    tag_data = next(
        (r for r in results if compare_magnet_links(str(r.magnet_link), str(download.magnet_uri))),
        None,
    )

    if tag_data is None:
        logger.info('No tag data for currently running download: %s', download.name)
        return False

    all_files_recursive = list_files_recursive(source_path.absolute().as_posix())

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
        logger.info('Processed all files in directory: %s', source_path)
        return True

    logger.error('Failure to process all files in directory: %s', source_path)
    return False


def _qbt_background_job(download: QbtDownloadData, job: Job, session: Session):
    source_path = Path(download.content_path)
    if source_path.is_dir():
        should_delete_download = _save_directory_files(source_path, download, session, logger)
    else:
        should_delete_download = _save_file(source_path, download, session, logger)

    if should_delete_download:
        delete_download_sync(download.hash)


async def qbt_background_service():
    while True:
        try:
            session: Session = get_session_non_gen()
            downloads = await list_downloads()
            for download in downloads:
                job_key = f'QBT-{download.name}'

                if (
                    _download_finished(download)
                    and not _media_entry_exists(download, session)
                    and not _job_exists(job_key, session)
                ):
                    process_scoped_session = get_new_session()

                    args: tuple[QbtDownloadData] = (download,)

                    job = await start_job(
                        job_key,
                        f'QBT download of: {download.name}',
                        _qbt_background_job,
                        process_scoped_session,
                        args,
                    )

                    logger.info('Started job: %s, %s', job_key, job.id)
            await asyncio.sleep(30)
        except Exception as e:
            logger.exception('Error in background service: %s', str(e))
        finally:
            if session:  # type: ignore
                session.close()
