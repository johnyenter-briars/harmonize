from pathlib import Path

import requests


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
