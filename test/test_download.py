import requests


def test_download_youtube_video(setup):
    url = f"{setup['base_url']}/youtube/video/A_MjCqQoLLA"

    response = requests.post(url)
    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'


def test_download_youtube_playlist(setup):
    url = f"{setup['base_url']}/youtube/playlist/PLHTo__bpnlYUWoJwsRJMtLypal4yovviQ"

    response = requests.post(url)
    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'
