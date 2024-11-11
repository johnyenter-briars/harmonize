import logging

from fastapi import APIRouter

from harmonize.defs.qbt import (
    AddDownloadRequest,
    DeleteDownloadsRequest,
    PauseDownloadsRequest,
    QbtDownloadData,
    ResumeDownloadsRequest,
)
from harmonize.defs.response import BaseResponse
from harmonize.qbt import qbt

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/qbt')


@router.get('/list', status_code=200)
async def list_torrents() -> BaseResponse[list[QbtDownloadData]]:
    files: list[QbtDownloadData] = await qbt.list_downloads()

    return BaseResponse[list[QbtDownloadData]](message='Found', status_code=200, value=files)


@router.post('/add', status_code=201)
async def add_torrent(request: AddDownloadRequest) -> BaseResponse[None]:
    for magnet_link in request.magnet_links:
        _ = await qbt.add_torrent(magnet_link)

    return BaseResponse[None](message='Added', status_code=201, value=None)


@router.post('/resume', status_code=201)
async def resume_torrents(request: ResumeDownloadsRequest) -> BaseResponse[None]:
    for torrent_hash in request.hashes:
        _ = await qbt.resume_download(torrent_hash)

    return BaseResponse[None](message='Resumed', status_code=201, value=None)


@router.post('/pause', status_code=201)
async def pause_torrents(request: PauseDownloadsRequest) -> BaseResponse[None]:
    for torrent_hash in request.hashes:
        _ = await qbt.pause_torrent(torrent_hash)

    return BaseResponse[None](message='Paused', status_code=201, value=None)


@router.post('/delete', status_code=201)
async def delete_torrents(request: DeleteDownloadsRequest) -> BaseResponse[None]:
    for torrent_hash in request.hashes:
        _ = await qbt.delete_download(torrent_hash)

    return BaseResponse[None](message='Deleted', status_code=201, value=None)
