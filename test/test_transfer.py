import requests


def test_trasfer_video_0(setup):
    url = f"{setup['base_url']}/media/video"

    response = requests.get(url)

    current_media = response.json()

    id = current_media['value'][0]['id']

    url = f"{setup['base_url']}/transfer/mediasystem/{id}"

    response = requests.post(url)

    assert response.status_code == 201, f'Expected status code 200, but got {response.status_code}'


def test_trasfer_video_1(setup):
    url = f"{setup['base_url']}/media/video"

    response = requests.get(url)

    current_media = response.json()

    id = current_media['value'][1]['id']

    url = f"{setup['base_url']}/transfer/mediasystem/{id}"

    response = requests.post(url)

    assert response.status_code == 201, f'Expected status code 200, but got {response.status_code}'


def test_trasfer_audio_0(setup):
    url = f"{setup['base_url']}/media/audio"

    response = requests.get(url)

    current_media = response.json()

    id = current_media['value'][0]['id']

    url = f"{setup['base_url']}/transfer/mediasystem/{id}"

    response = requests.post(url)

    assert response.status_code == 201, f'Expected status code 200, but got {response.status_code}'
