from fastapi import APIRouter

from harmonize.defs.playlist import Playlist

router = APIRouter(prefix='/api')


@router.get('/playlist/{playlist_name}')
async def get_playlist(playlist_name: str) -> Playlist:
    return {
        'files': [
            'Lo-fi Hip Hop 30 second Loop.mp3',
            'Sense.mp3',
            'Lo-fi Hip Hop 30 second Loop.mp3',
        ]
    }
