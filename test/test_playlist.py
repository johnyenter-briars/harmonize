import requests


def test_get_playlist(setup):
    url = f"{setup['base_url']}/playlist/foo"

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'
