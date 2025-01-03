import uuid

from fastapi import APIRouter, Depends
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, Playlist, PlaylistWithMediaEntries
from harmonize.defs.response import BaseResponse

router = APIRouter(prefix='/api')


@router.post('/playlist/{playlist_name}')
async def add_playlist(
    playlist_name: str, session: Session = Depends(get_session)
) -> BaseResponse[Playlist]:
    new_playlist = Playlist(name=playlist_name)

    session.add(new_playlist)
    session.commit()

    return BaseResponse[Playlist](message='Created', status_code=201, value=new_playlist)


@router.put('/playlist/{playlist_id}/entry/{media_entry_id}')
async def add_entry_to_playlist(
    playlist_id: uuid.UUID, media_entry_id: uuid.UUID, session: Session = Depends(get_session)
) -> BaseResponse[Playlist]:
    media_entry = session.get(MediaEntry, media_entry_id)
    playlist = session.get(Playlist, playlist_id)

    if media_entry is None:
        return BaseResponse[Playlist](message='Media entry not found', status_code=404, value=None)

    if playlist is None:
        return BaseResponse[Playlist](message='Playlist not found', status_code=404, value=None)

    playlist.media_entries.append(media_entry)

    session.commit()

    return BaseResponse[Playlist](
        message='Entry added to playlist', status_code=201, value=playlist
    )


@router.get('/playlist/{playlist_name}')
async def get_playlist(
    playlist_name: str, session: Session = Depends(get_session)
) -> BaseResponse[Playlist]:
    return BaseResponse[Playlist](message='Found', status_code=201, value=None)


# @router.get('/playlist')
# async def get_playlists(session: Session = Depends(get_session)) -> BaseResponse[list[Playlist]]:
#     statement = select(Playlist).options(selectinload(Playlist.media_entries))
#     playlists = list(session.exec(statement))
#     return BaseResponse[list[Playlist]](message='Found', status_code=201, value=playlists)


@router.get('/playlist', response_model=list[PlaylistWithMediaEntries])
async def get_playlists(session: Session = Depends(get_session)) -> list[Playlist]:
    statement = select(Playlist)
    playlists = list(session.exec(statement))

    return playlists
