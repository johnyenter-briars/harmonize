import requests


def test_trasfer_file(setup):
    url = f"{setup['base_url']}/media/video"

    response = requests.get(url)

    current_media = response.json()

    id = current_media['value'][0]['id']

    url = f"{setup['base_url']}/transfer/mediasystem/{id}"

    response = requests.post(url)

    assert response.status_code == 201, f'Expected status code 200, but got {response.status_code}'
