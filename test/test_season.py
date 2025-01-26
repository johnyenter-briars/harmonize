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


def test_disassociate_media_entries(setup, season_id, media_entry_ids):
    url = f'{setup["base_url"]}/season/{season_id}/disassociate'
    payload = {'media_entry_ids': media_entry_ids}

    response = requests.post(url, json=payload)

    assert response.status_code == 200, f'Expected status code 200, got {response.status_code}'
    assert response.json()['message'] == 'Media entries disassociated successfully'


def test_get_season_details(setup, season_id):
    url = f'{setup["base_url"]}/season/{season_id}'

    response = requests.get(url)

    assert response.status_code == 200, f'Expected status code 200, got {response.status_code}'
    assert response.json()['message'] == 'Season details retrieved successfully'
    assert 'media_entries' in response.json()['value']
