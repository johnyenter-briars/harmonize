import logging

from fastapi import APIRouter, Depends, Query
from sqlmodel import Session, select

from harmonize.const import MEDIA_ROOT, MUSIC_ROOT_LEGACY
from harmonize.db.database import get_session
from harmonize.db.models import Job

logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api')


@router.get('/media')
async def list_media() -> dict[str, list[str]]:
    albums: dict[str, list[str]] = {}
    for item in MEDIA_ROOT.iterdir():
        logger.info(item)
        if item.is_dir():
            albums[item.name] = [song.name for song in item.iterdir()]
    return albums


@router.get('/list_music')
async def list_music() -> dict[str, list[str]]:
    albums: dict[str, list[str]] = {}
    for item in MUSIC_ROOT_LEGACY.iterdir():
        if item.is_dir():
            albums[item.name] = [song.name for song in item.iterdir()]
    return albums


@router.get('/list_jobs', response_model=list[Job])
async def list_jobs(
    *,
    session: Session = Depends(get_session),
    offset: int = 0,
    limit: int = Query(default=100, le=100),
):
    jobs = session.exec(select(Job).offset(offset).limit(limit)).all()
    return jobs
