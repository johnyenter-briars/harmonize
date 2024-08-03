import logging

from fastapi import APIRouter

from harmonize.const import MEDIA_ROOT, MUSIC_ROOT_LEGACY

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
