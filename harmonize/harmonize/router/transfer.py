import logging

from fastapi import APIRouter, Depends
from sqlmodel import Session

import harmonize.config
import harmonize.config.harmonizeconfig
import harmonize.config.harmonizesecrets
from harmonize.db.database import get_session
from harmonize.db.models import Job
from harmonize.defs.response import BaseResponse
from harmonize.job.methods import start_job
from harmonize.transfer.mediasystem import transfer_file

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG
secrets = harmonize.config.harmonizesecrets.HARMONIZE_SECRETS


logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/transfer')


@router.post('/mediasystem/{media_entry_id}', status_code=201)
async def transfer_file_mediasystem(
    media_entry_id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[None]:
    args: tuple[str] = ('foo',)

    job = await start_job(f'Transfer of file: {"foo"}', _transfer_file_job, session, args)

    return BaseResponse[None](message='File transfer started', status_code=200, value=None)


def _transfer_file_job(
    foo: str,
    job: Job,
    session: Session,
):
    local_file = (
        '/home/john/Downloads/SpiderMan No Way Home 2021 1080p HD-TS V3 Line Audio x264 AAC.mkv'
    )
    remote_file = (
        '/storage/videos/SpiderMan No Way Home 2021 1080p HD-TS V3 Line Audio x264 AAC.mkv'
    )

    transfer_file(
        secrets.media_system_ip,
        secrets.media_system_username,
        secrets.media_system_password,
        local_file,
        remote_file,
    )
