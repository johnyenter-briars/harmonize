import requests


def test_get_job(setup):
    url = f"{setup['base_url']}/job/d2b4eb79-3855-41ac-be1c-a4e38d19c413"

    response = requests.get(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'


def test_cancel_job(setup):
    url = f"{setup['base_url']}/job/cancel/f3abbc50-cdc9-4afd-b6b7-8b93d8bd02fc"

    response = requests.post(url)
    assert response.status_code == 200, f'Expected status code 200, but got {response.status_code}'
