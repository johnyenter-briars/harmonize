import datetime
import io
import logging
import uuid
from pathlib import Path
from typing import Final, Literal, cast

import requests
from fastapi import APIRouter, Depends
from fastapi.responses import FileResponse
from mutagen.easyid3 import EasyID3
from mutagen.mp3 import MP3
from PIL import Image
from PIL.ImageFile import ImageFile
from sqlmodel import Session, select

from harmonize.const import (
    AUDIO_ROOT,
    COVERART_ARCHIVE_ROOT,
    MUSTICBRAINZ_RELEASE_ROOT,
    TMP_ALBUM_ART_DIR,
)
from harmonize.db.database import get_session
from harmonize.db.models import MediaElementSource, MediaEntry, MediaEntryType
from harmonize.defs.metadata import ApicData, HarmonizeThumbnail, MediaMetadata
from harmonize.defs.musicbrainz import (
    CoverArtArchiveResponse,
    MusicBrainzReleaseResponse,
)
from harmonize.defs.response import BaseResponse
from harmonize.util.metadata import get_album_artwork_itunes

logger = logging.getLogger('harmonize')

THUMBNAIL_SIZES: Final = (1200, 500, 250)

router = APIRouter(prefix='/api')


def get_musicbrainz_releases(
    *,
    album: str | None = None,
    song: str | None = None,
    artist: str | None = None,
) -> MusicBrainzReleaseResponse:
    def add_param(query_parameters: str | None, param: str) -> str:
        if query_parameters is None:
            return param

        return query_parameters + f' AND {param}'

    query_parameters: str | None = None
    if album:
        query_parameters = add_param(query_parameters, f'release:{album}')
    if song:
        query_parameters = add_param(query_parameters, f'recording:{song}')
    if artist:
        query_parameters = add_param(query_parameters, f'artist:{song}')

    response = requests.get(
        MUSTICBRAINZ_RELEASE_ROOT.format(query_parameters=query_parameters), timeout=10
    )

    response.raise_for_status()

    return response.json()


def find_album_art_thumbails(
    *,
    album: str | None = None,
    song: str | None = None,
    artist: str | None = None,
) -> HarmonizeThumbnail:
    musicbrainz_releases = get_musicbrainz_releases(
        album=album,
        song=song,
        artist=artist,
    )

    first_match_mbid = musicbrainz_releases.get('releases')[0].get('id')

    cover_art_url = f'{COVERART_ARCHIVE_ROOT}/{first_match_mbid}'
    response = requests.get(cover_art_url, timeout=10)
    response.raise_for_status()
    cover_art_response = cast(CoverArtArchiveResponse, response.json())
    thumbnails = cover_art_response.get('images')[0].get('thumbnails')
    return {
        'xl': thumbnails.get('1200'),  # type: ignore[reportReturnType]
        'large': thumbnails.get('large'),
        'small': thumbnails.get('small'),
    }


def make_thumbnails(album_dir: Path, original_im: ImageFile):
    for tn_size in THUMBNAIL_SIZES:
        tn_tup = (tn_size, tn_size)
        try:
            with original_im.copy() as im:
                # Scale up original image if smaller than necessary thumbnail size
                if original_im.size[0] < tn_size:
                    im.resize(tn_tup)
                else:
                    im.thumbnail(tn_tup)
                im.save(album_dir / f'{tn_size}.png')
        except OSError:
            logger.exception('Failed to create thumbnail for size', extra={'size': tn_size})


@router.get('/metadata/media/{id}')
async def media_metadata(
    id: uuid.UUID, session: Session = Depends(get_session)
) -> BaseResponse[MediaMetadata]:
    statement = select(MediaEntry).where(MediaEntry.id == id)
    media_entry = session.exec(statement).first()

    if media_entry is None:
        return BaseResponse[MediaMetadata](
            message='Media entry not found', status_code=404, value=None
        )

    absolute_path = (AUDIO_ROOT / 'Sense.mp3').absolute().as_posix()
    media_entry = MediaEntry(
        name='Sense.mp3',
        absolute_path=absolute_path,
        source=MediaElementSource.YOUTUBE,
        youtube_id='',
        type=MediaEntryType.AUDIO,
        date_added=datetime.datetime.now(datetime.UTC),
        magnet_link=None,
    )

    track = MP3(Path(media_entry.absolute_path))
    tags = EasyID3(Path(media_entry.absolute_path))

    album_name = _get_str_tag(tags, 'album')

    if album_name is None:
        album_dir = TMP_ALBUM_ART_DIR / media_entry.name
    else:
        album_dir = TMP_ALBUM_ART_DIR / album_name

    img_data = cast(ApicData | None, track.get('APIC:'))

    # TODO
    if img_data:
        # Make album art dir in case it doesn't exist yet
        album_dir.mkdir(parents=True, exist_ok=True)
        with Image.open(io.BytesIO(img_data.data)) as original_im:
            make_thumbnails(album_dir, original_im)
        url_base = f'album_art/{album_name}'
        thumbnail = HarmonizeThumbnail(
            xl=f'{url_base}/1200', large=f'{url_base}/500', small=f'{url_base}/250'
        )
    else:
        # Retrieve album art if no art is included in the file
        thumbnail: HarmonizeThumbnail = find_album_art_thumbails(song=media_entry.name)
    (small_thumbnail, xl_thumbnail) = get_album_artwork_itunes('Singata (Mystic Queen)')

    # download_image(xl_thumbnail, TMP_ALBUM_ART_DIR / f'{media_entry.id}.jpg')

    thumbnail = HarmonizeThumbnail(xl=xl_thumbnail, small=small_thumbnail, large='')

    return BaseResponse[MediaMetadata](
        message='Got metadata',
        status_code=200,
        value=MediaMetadata(
            title='foo',
            album='bar',
            artist='bing',
            artwork=thumbnail,
        ),
    )


def _get_str_tag(tags: EasyID3, tag: Literal['title', 'album', 'artist']) -> str | None:
    if tags.get(tag) is None:
        return None

    return cast(list[str], tags.get(tag))[0]


# @router.get('/album_art/{album}/{size}')
# async def album_art(album: str, size: Literal['250', '500', '1200']) -> FileResponse:
#     return FileResponse(TMP_ALBUM_ART_DIR / album / f'{size}.png')


@router.get('/album_art/{id}')
async def album_art(id: str) -> FileResponse:
    return FileResponse(TMP_ALBUM_ART_DIR / f'{id}.jpg')
