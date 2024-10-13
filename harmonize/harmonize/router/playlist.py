from fastapi import APIRouter

from harmonize.defs.playlist import Playlist

router = APIRouter(prefix='/api')


@router.get('/playlist/{playlist_name}')
async def get_playlist(playlist_name: str) -> Playlist:
    return Playlist(files=[])
