import requests


def test_download_youtube(setup):
    url = f"{setup['base_url']}/download/youtube/y5auOZPxUs0"

    response = requests.post(url)
    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'
