import logging
import uuid

from fastapi import APIRouter, Depends
from sqlmodel import Session

from harmonize.db.database import get_session
from harmonize.db.models import QbtDownloadTagInfo
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
async def add_torrent(
    request: AddDownloadRequest,
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    for magnet_link in request.magnet_links:
        tag_info = QbtDownloadTagInfo(
            id=uuid.uuid4(),
            name=request.name,
            magnet_link=magnet_link,
            type=request.type,
            video_type=request.video_type,
            audio_type=request.audio_type,
            create_season=request.create_season,
        )
        session.add(tag_info)

        _ = await qbt.add_torrent(magnet_link)

        session.commit()

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
