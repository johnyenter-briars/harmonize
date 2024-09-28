import requests

public_ip = '0.0.0.0'
port = 8000


def test_search_piratebay():
    url = f'http://{public_ip}:{port}/api/search/piratebay/starwars'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'

    data = response.json()

    assert isinstance(data['value'], list), 'Response data should be a list'


def test_search_t1337x():
    url = f'http://{public_ip}:{port}/api/search/xt1337/starwars'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'

    data = response.json()

    assert isinstance(data['value'], list), 'Response data should be a list'
