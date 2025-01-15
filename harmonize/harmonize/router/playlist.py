import uuid

from fastapi import APIRouter, Depends
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import MediaEntry, Playlist
from harmonize.defs.response import BaseResponse

router = APIRouter(prefix='/api/playlist')


@router.post('/{playlist_name}', status_code=201)
async def add_playlist(
    playlist_name: str, session: Session = Depends(get_session)
) -> BaseResponse[Playlist]:
    new_playlist = Playlist(name=playlist_name)

    session.add(new_playlist)
    session.commit()

    return BaseResponse[Playlist](message='Created', status_code=201, value=new_playlist)


@router.put('/{playlist_id}/entry/{media_entry_id}', status_code=201)
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
    session.refresh(playlist)

    return BaseResponse[Playlist](
        message='Entry added to playlist', status_code=201, value=playlist
    )


@router.get('/{playlist_name}')
async def get_playlist(
    playlist_name: str, session: Session = Depends(get_session)
) -> BaseResponse[Playlist]:
    return BaseResponse[Playlist](message='Found', status_code=201, value=None)


@router.get('/')
async def get_playlists(session: Session = Depends(get_session)) -> BaseResponse[list[Playlist]]:
    statement = select(Playlist)
    playlists = list(session.exec(statement))
    return BaseResponse[list[Playlist]](message='Found', status_code=201, value=playlists)


@router.get('/entries/{playlist_id}')
async def get_entries_for_playlist(
    playlist_id: uuid.UUID, session: Session = Depends(get_session)
) -> BaseResponse[list[MediaEntry]]:
    statement = select(Playlist).where(Playlist.id == playlist_id)
    entries = next(session.exec(statement)).media_entries
    return BaseResponse[list[MediaEntry]](message='Found', status_code=201, value=entries)
