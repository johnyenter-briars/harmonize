import base64

import pytest


@pytest.fixture
def setup():
    print('Running setup...')
    server_ip = 'http://127.0.0.1'
    port = 8000

    credentials = base64.b64encode(b'john:test123').decode('utf-8')

    headers = {'Authorization': f'Basic {credentials}', 'Content-Type': 'application/json'}

    return {'base_url': f'{server_ip}:{port}/api', 'headers': headers}
