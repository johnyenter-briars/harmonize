import subprocess
from datetime import datetime
from pathlib import Path

from fastapi import APIRouter, Request

import harmonize.config
import harmonize.config.harmonizeconfig
from harmonize.defs.health import Drive, HealthStatus, Uptime
from harmonize.defs.response import BaseResponse
from harmonize.file.drive import get_folder_size_bytes

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG

router = APIRouter(prefix='/api/health')


def _vpn_status() -> tuple[bool, str]:
    output = subprocess.check_output(
        'nordvpn status | grep Status',
        shell=True,  # Let this run in the shell
        stderr=subprocess.STDOUT,
    )

    nord_status = str(output).split(': ')[1]

    country = ''
    connected = False
    if 'Connected' in nord_status:
        output = subprocess.check_output(
            'nordvpn status | grep Country',
            shell=True,  # Let this run in the shell
            stderr=subprocess.STDOUT,
        )

        country = str(output).split(': ')[1].replace('\\n', '').replace("'", '')
        connected = True

    return (connected, country)


@router.get('/status')
async def status(request: Request) -> BaseResponse[HealthStatus]:
    start_time = request.app.state.start_time
    current_time = datetime.now()
    uptime = current_time - start_time
    uptime_seconds = int(uptime.total_seconds())
    hours, remainder = divmod(uptime_seconds, 3600)
    minutes, seconds = divmod(remainder, 60)

    uptime = Uptime(seconds=uptime_seconds, hours=hours, minutes=minutes)

    drives = [
        Drive(path=drive, space_used=round(get_folder_size_bytes(Path(drive)) / (1024**3), 2))
        for drive in config.drives
    ]

    (connected, country) = _vpn_status()

    status = HealthStatus(
        uptime=uptime, drives=drives, vpn_connected=connected, vpn_country=country
    )

    return BaseResponse[HealthStatus](message='Status', status_code=200, value=status)
