import logging

from fastapi import APIRouter

from harmonize.defs.qbt import (
    AddTorrentRequest,
    DeleteTorrentsRequest,
    PauseTorrentsRequest,
    ResumeTorrentsRequest,
    TorrentData,
)
from harmonize.defs.response import BaseResponse
from harmonize.qbt import qbt

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/qbt')


@router.get('/list', status_code=200)
async def list_torrents() -> BaseResponse[list[TorrentData]]:
    files: list[TorrentData] = await qbt.list_torrents()

    return {'status_code': 200, 'message': 'Status found', 'value': files}


@router.post('/add', status_code=201)
async def add_torrent(request: AddTorrentRequest) -> BaseResponse[None]:
    # TODO: add check - or have some other way of detecting VPN
    # if fsconfig.CONFIG['check-vpn'] and not vpn_running()[0]:
    #     print('VPN is not running!')
    #     raise web.HTTPInternalServerError

    for magnet_link in request.magnet_links:
        _ = await qbt.add_torrent(magnet_link)

    return {'status_code': 201, 'message': 'Torrent added', 'value': None}


@router.post('/resume', status_code=201)
async def resume_torrents(request: ResumeTorrentsRequest) -> BaseResponse[None]:
    for torrent_hash in request.hashes:
        _ = await qbt.resume_torrent(torrent_hash)

    return {'status_code': 201, 'message': 'Torrents resumed', 'value': None}


@router.post('/pause', status_code=201)
async def pause_torrents(request: PauseTorrentsRequest) -> BaseResponse[None]:
    for torrent_hash in request.hashes:
        _ = await qbt.pause_torrent(torrent_hash)

    return {'status_code': 201, 'message': 'Torrents paused', 'value': None}


@router.post('/delete', status_code=201)
async def delete_torrents(request: DeleteTorrentsRequest) -> BaseResponse[None]:
    for torrent_hash in request.hashes:
        _ = await qbt.delete_torrent(torrent_hash)

    return {'status_code': 201, 'message': 'Torrents deleted', 'value': None}
