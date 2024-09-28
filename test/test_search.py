import requests


def test_search_piratebay(setup):
    url = f'{setup['base_url']}/search/piratebay/starwars'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'

    data = response.json()

    assert isinstance(data['value'], list), 'Response data should be a list'


def test_search_t1337x(setup):
    url = f'{setup['base_url']}/search/xt1337/starwars'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'

    data = response.json()

    assert isinstance(data['value'], list), 'Response data should be a list'


def test_search_youtube(setup):
    url = f"{setup['base_url']}/search/youtube/Lo-fi Hip Hop 30 second Loop"

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'
    data = response.json()
    assert isinstance(data['value'], list), 'Expected a list of search results'
