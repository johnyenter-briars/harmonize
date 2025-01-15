import requests


def test_add_playlist(setup):
    url = f"{setup['base_url']}/playlist/my new playlist 2"

    response = requests.post(url)
    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'


def test_get_playlist(setup):
    url = f"{setup['base_url']}/playlist/my new playlist"

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'


def test_add_element_to_playlist(setup):
    url = f"{setup['base_url']}/playlist"

    response = requests.get(url)

    all_playlists = (response.json())['value']

    first_playlist = all_playlists[0]
    first_playlist_id = first_playlist['id']

    url = f"{setup['base_url']}/media/audio"

    response = requests.get(url)

    all_media_entries = (response.json())['value']

    first_media_entry = all_media_entries[2]
    first_media_entry_id = first_media_entry['id']

    url = f"{setup['base_url']}/playlist/{first_playlist_id}/entry/{first_media_entry_id}"

    response = requests.put(url)

    assert response.status_code == 201, f'Expected status code 201, but got {response.status_code}'
