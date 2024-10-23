import pytest


@pytest.fixture()
def setup():
    print('Running setup...')
    server_ip = 'http://127.0.0.1'
    port = 8000
    headers = {'Content-Type': 'application/json'}

    return {'base_url': f'{server_ip}:{port}/api', 'headers': headers}
