import io
import logging
from pathlib import Path
from typing import Final, Literal, cast

import requests
from fastapi import APIRouter
from fastapi.responses import FileResponse
from mutagen.easyid3 import EasyID3
from mutagen.mp3 import MP3
from PIL import Image
from PIL.ImageFile import ImageFile

from harmonize.const import MUSIC_ROOT, TMP_ALBUM_ART_DIR
from harmonize.defs.metadata import ApicData, HarmonizeThumbnail, MediaMetadata
from harmonize.defs.musicbrainz import (
    CoverArtArchiveResponse,
    MusicBrainzReleaseResponse,
)

logger = logging.getLogger('harmonize')
COVERART_ARCHIVE_ROOT: Final = 'http://coverartarchive.org/release'

MUSTICBRAINZ_RELEASE_ROOT: Final = (
    'https://musicbrainz.org/ws/2/release/?query={query_parameters}&fmt=json'
)

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


@router.get('/metadata/media/{filename}')
async def media_metadata(filename: str) -> MediaMetadata:
    track = MP3(MUSIC_ROOT / filename)
    tags = EasyID3(MUSIC_ROOT / filename)

    album_name = _get_str_tag(tags, 'album')
    album_dir = TMP_ALBUM_ART_DIR / album_name
    img_data = cast(ApicData | None, track.get('APIC:'))

    thumbnails: HarmonizeThumbnail
    if img_data:
        # Make album art dir in case it doesn't exist yet
        album_dir.mkdir(parents=True, exist_ok=True)
        with Image.open(io.BytesIO(img_data.data)) as original_im:
            make_thumbnails(album_dir, original_im)
        url_base = f'album_art/{album_name}'
        thumbnails = {
            'xl': f'{url_base}/1200',
            'large': f'{url_base}/500',
            'small': f'{url_base}/250',
        }
    else:
        # Retrieve album art if no art is included in the file
        thumbnails = find_album_art_thumbails(album=album_name)

    return {
        'title': _get_str_tag(tags, 'title'),
        'album': album_name,
        'artist': _get_str_tag(tags, 'artist'),
        'artwork': thumbnails,
    }


def _get_str_tag(tags: EasyID3, tag: Literal['title', 'album', 'artist']) -> str:
    return cast(list[str], tags.get(tag))[0]


@router.get('/album_art/{album}/{size}')
async def album_art(album: str, size: Literal['250', '500', '1200']) -> FileResponse:
    return FileResponse(TMP_ALBUM_ART_DIR / album / f'{size}.png')
