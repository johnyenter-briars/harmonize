import requests


def test_download_youtube_video(setup):
    videos = [
        'YEESfv-11ng',  # Get Back (Remastered 2015)
    ]

    url = f"{setup['base_url']}/youtube/video/{videos[0]}"

    response = requests.post(url)
    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'


def test_download_youtube_playlist(setup):
    url = f"{setup['base_url']}/youtube/playlist/PLycVTiaj8OI9Ptzsl9nzxfwUgZ0jx-Hyb"

    response = requests.post(url)
    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'
