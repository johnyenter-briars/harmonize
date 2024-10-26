from io import BytesIO
from pathlib import Path
from typing import cast

import requests
from bs4 import BeautifulSoup
from PIL import Image

from harmonize.const import COVERART_ARCHIVE_ROOT, MUSTICBRAINZ_RELEASE_ROOT
from harmonize.defs.metadata import HarmonizeThumbnail
from harmonize.defs.musicbrainz import CoverArtArchiveResponse, MusicBrainzReleaseResponse


def get_album_artwork_itunes(song_title: str) -> tuple[str, str]:
    escaped = song_title.replace(' ', '+')
    search_url = f'https://itunes.apple.com/search?term={escaped}&limit=20'
    response = requests.get(search_url)

    if response.status_code == 200:
        data = response.json()
        if data['resultCount'] > 0:
            for result in data['results']:
                track_name = result.get('trackName').lower()
                if track_name in song_title.lower():
                    target_result = result

            # TODO: fix this
            album_artwork_url = target_result.get('artworkUrl100')  # type: ignore

            if album_artwork_url:
                large_artwork_url = album_artwork_url.replace('100x100', '600x600')
                small = album_artwork_url
                return (small, large_artwork_url)
            else:
                return ('No artwork available', '')
        else:
            return ('No results found', '')
    else:
        return (f'Error fetching data: {response.status_code}', '')


def download_image(url, save_path) -> bool:
    response = requests.get(url)
    if response.status_code == 200:
        save_path = Path(save_path)
        with save_path.open('wb') as file:
            file.write(response.content)
        print(f'Image saved to: {save_path}')
        return True
    else:
        print(f'Failed to download image. Status code: {response.status_code}')
        return False


def get_album_art_bing(song_name, artist_name=None):
    # Prepare search query (adding artist name if provided)
    query = (
        f'{song_name} album cover' if not artist_name else f'{song_name} {artist_name} album cover'
    )
    url = f"https://www.bing.com/images/search?q={query.replace(' ', '+')}&FORM=HDRSC2"

    # Send request to Bing Images
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
    }
    response = requests.get(url, headers=headers)
    response.raise_for_status()  # Raise error for bad status

    # Parse the HTML
    soup = BeautifulSoup(response.text, 'html.parser')
    img_tags = soup.find_all('img', class_='mimg')

    if not img_tags:
        print('No images found for this song.')
        return None

    img_url = None
    for img_tag in img_tags:
        img_src = img_tag.get('src') or img_tag.get('data-src')
        if img_src and (
            img_src.endswith('.jpg') or img_src.endswith('.jpeg') or img_src.endswith('.png')
        ):
            img_url = img_src
            break

    if not img_url:
        print('No suitable images found for this song.')
        return None

    # Download and save the image
    img_response = requests.get(img_url)
    img = Image.open(BytesIO(img_response.content))
    filename = f'{song_name}_album_art.jpg'
    img.save(filename, 'JPEG')  # Save image as JPEG file
    print(f'Album art saved as: {filename}')

    # img.show()  # Displays the image
    img.save(f'{song_name}_album_art.jpg')

    return img_url


def _get_musicbrainz_releases(
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


def get_album_art_musicbrainz(
    *,
    album: str | None = None,
    song: str | None = None,
    artist: str | None = None,
) -> HarmonizeThumbnail:
    musicbrainz_releases = _get_musicbrainz_releases(
        album=album,
        song=song,
        artist=artist,
    )

    first_match_mbid = musicbrainz_releases.get('releases')[1].get('id')

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
