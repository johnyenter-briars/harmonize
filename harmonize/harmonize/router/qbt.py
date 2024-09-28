import logging

from fastapi import APIRouter

from harmonize.defs.addtorrentrequest import AddTorrentRequest
from harmonize.defs.response import BaseResponse
from harmonize.qbt import qbt

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/qbt')


@router.get('/list', status_code=200)
async def list_torrents() -> BaseResponse[list[dict]]:
    files: list[dict] = await qbt.list_torrents()

    return {'status_code': 200, 'message': 'success', 'value': files}


@router.post('/add', status_code=201)
async def add_torrent(request: AddTorrentRequest) -> BaseResponse[None]:
    # TODO: add check - or have some other way of detecting VPN
    # if fsconfig.CONFIG['check-vpn'] and not vpn_running()[0]:
    #     print('VPN is not running!')
    #     raise web.HTTPInternalServerError

    for magnet_link in request.magnet_links:
        _ = await qbt.add_torrent(magnet_link)

    return {'status_code': 201, 'message': 'success', 'value': None}


# @routes.post('/api/torrents/resume')
# async def _resume_torrents(request):
#     r = await request.json()

#     for hash in r['hashes']:
#         _ = await resume_torrent(hash)

#     return web.json_response({'message': 'torrents resumed'})


# @routes.post('/api/torrents/pause')
# async def pause_torrents(request):
#     r = await request.json()

#     for hash in r['hashes']:
#         _ = await pause_torrent(hash)

#     return web.json_response({'message': 'torrents paused'})


# @routes.post('/api/torrents/delete')
# async def _delete_torrents(request):
#     r = await request.json()

#     for hash in r['hashes']:
#         _ = await delete_torrent(hash)

#     return web.json_response({'message': 'torrents deleted'})
