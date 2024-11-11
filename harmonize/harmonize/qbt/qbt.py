import asyncio
import datetime
import logging
from codecs import encode

import aiohttp
import pydantic
from fastapi import Depends
from sqlmodel import Session, select

from harmonize.config.harmonizeconfig import HARMONIZE_CONFIG
from harmonize.db.database import get_session
from harmonize.db.models import MediaElementSource, MediaElementType, MediaEntry
from harmonize.defs.qbt import QbtDownloadData

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


def _ready_for_media_entry(data: QbtDownloadData) -> bool:
    return data.state == 'stalledUp'


def _media_entry_exists(
    data: QbtDownloadData,
    session: Session = Depends(get_session),
) -> bool:
    statement = select(MediaEntry).where(MediaEntry.magnet_link == data.magnet_uri)
    media_entries = session.exec(statement).all()
    return False


async def qbt_background_service(
    session: Session = Depends(get_session),
):
    while True:
        try:
            downloads = await list_downloads()
            for download in downloads:
                if _ready_for_media_entry(download) and not _media_entry_exists(download):
                    media_entry = MediaEntry(
                        name=download.name,
                        absolute_path=download.content_path,
                        source=MediaElementSource.MAGNETLINK,
                        youtube_id=None,
                        magnet_link=download.magnet_uri,
                        type=MediaElementType.VIDEO,
                        date_added=datetime.datetime.now(datetime.UTC),
                    )
                    # session.add(media_entry)

                    logger.debug('Added media entry: %s', media_entry.id)
            logger.info('Background service is running...')
            await asyncio.sleep(5)
        except Exception as e:
            logger.exception('Error in background service: %s', str(e))
