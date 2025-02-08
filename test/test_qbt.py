import requests


def test_list_torrents(setup):
    url = f'{setup["base_url"]}/qbt/list'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'

    data = response.json()

    assert isinstance(data['value'], list), 'Response data should be a list'


def test_add_torrent(setup):
    url = f'{setup["base_url"]}/search/piratebay/spiderman'

    response = requests.get(url)

    magnet_links_response = response.json()

    url = f'{setup["base_url"]}/qbt/add'

    request_data = {
        'magnetLinks': [magnet_links_response['value'][1]['magnetLink']],
        'type': 1,
        'videoType': 0,
        'audioType': None,
        'createSeason': None,
    }

    response = requests.post(url, json=request_data)

    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'


def test_pause_torrent(setup):
    test_add_torrent(setup)

    url = f'{setup["base_url"]}/qbt/list'

    response = requests.get(url)

    data = response.json()

    first_torrent = data['value'][0]

    torrent_hash = first_torrent['hash']

    url = f'{setup["base_url"]}/qbt/pause'

    request_data = {'hashes': [torrent_hash]}

    response = requests.post(url, json=request_data)

    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'


def test_resume_torrent(setup):
    test_add_torrent(setup)

    url = f'{setup["base_url"]}/qbt/list'

    response = requests.get(url)

    data = response.json()

    first_torrent = data['value'][0]

    torrent_hash = first_torrent['hash']

    url = f'{setup["base_url"]}/qbt/resume'

    request_data = {'hashes': [torrent_hash]}

    response = requests.post(url, json=request_data)

    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'


def test_delete_torrent(setup):
    test_add_torrent(setup)

    url = f'{setup["base_url"]}/qbt/list'

    response = requests.get(url)

    data = response.json()

    first_torrent = data['value'][0]

    torrent_hash = first_torrent['hash']

    url = f'{setup["base_url"]}/qbt/delete'

    request_data = {'hashes': [torrent_hash]}

    response = requests.post(url, json=request_data)

    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'
