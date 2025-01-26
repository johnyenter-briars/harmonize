import requests


def test_create_season(setup):
    url = f'{setup["base_url"]}/season/'

    response = requests.post(url, json={'name': 'test season again'})

    assert response.status_code == 201, f'Expected status code 201, got {response.status_code}'
    assert response.json()['message'] == 'Season created successfully'
    return response.json()['value']['id']


def test_associate_media_entries(setup):
    url = f'{setup["base_url"]}/media/video'
    response = requests.get(url, json={})
    data = response.json()
    entries = data['value']
    media_entry_ids = [entries[0]['id'], entries[1]['id']]

    url = f'{setup["base_url"]}/season'
    response = requests.get(url, json={})
    data = response.json()
    season_id = data['value'][4]['id']

    url = f'{setup["base_url"]}/season/associate'
    payload = {'seasonId': season_id, 'mediaEntryIds': media_entry_ids}

    response = requests.post(url, json=payload)

    assert response.status_code == 200, f'Expected status code 200, got {response.status_code}'
    assert response.json()['message'] == 'Media entries associated successfully'


def test_disassociate_media_entries(setup):
    url = f'{setup["base_url"]}/season/disassociate'
    payload = {
        'seasonId': '7e6448cb-4f58-4a17-b337-cb7f2116c2d5',
        'mediaEntryIds': ['2ab6a1d7-b9a1-4fe3-8398-ce4347f9e147'],
    }

    response = requests.post(url, json=payload)

    assert response.status_code == 200, f'Expected status code 200, got {response.status_code}'
    assert response.json()['message'] == 'Media entries disassociated successfully'


def test_get_season_details(setup):
    url = f'{setup["base_url"]}/season/7e6448cb-4f58-4a17-b337-cb7f2116c2d5'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, got {response.status_code}'
    assert response.json()['message'] == 'Season details retrieved successfully'
