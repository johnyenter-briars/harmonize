from fastapi import APIRouter

from harmonize.defs.playlist import Playlist

router = APIRouter(prefix='/api')


@router.get('/playlist/{playlistname}')
async def get_playlist(playlistname: str) -> Playlist:
    return {'files': ['Sense.mp3', 'Sense2.mp3', 'Sense3.mp3']}
