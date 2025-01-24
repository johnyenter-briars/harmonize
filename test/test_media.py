import requests


def test_metadata_media(setup):
    url = f'{setup["base_url"]}/metadata/media/Sense.mp3'

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'


def test_stream_media(setup):
    url = f'{setup["base_url"]}/stream/Sense.mp3'

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'


def test_list_music(setup):
    url = f'{setup["base_url"]}/media/music'

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'


def test_delete_entry(setup):
    url = f'{setup["base_url"]}/media/video'

    response = requests.get(url)

    data = response.json()

    id_to_delete = data['value'][0]['id']

    url = f'{setup["base_url"]}/media/{id_to_delete}'

    response = requests.delete(url)

    assert response.status_code == 200, f'Expected status code 204, but got {response.status_code}'
