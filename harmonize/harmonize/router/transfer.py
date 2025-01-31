import logging
import uuid
from pathlib import Path

from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select

import harmonize.config
import harmonize.config.harmonizeconfig
import harmonize.config.harmonizesecrets
from harmonize.db.database import get_session
from harmonize.db.models import Job, MediaEntry
from harmonize.defs.response import BaseResponse
from harmonize.defs.transferprogress import TransferDestination, TransferProgress
from harmonize.job.callback import start_job
from harmonize.transfer.mediasystem import get_all_running_transfers, transfer_file

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG
secrets = harmonize.config.harmonizesecrets.HARMONIZE_SECRETS


logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/transfer')


@router.post('/mediasystem/{media_entry_id}', status_code=201)
async def transfer_file_mediasystem(
    media_entry_id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    statement = select(MediaEntry).where(MediaEntry.id == uuid.UUID(media_entry_id))

    media_entries = session.exec(statement).all()

    if len(media_entries) == 0:
        raise HTTPException(status_code=404, detail="Can't find media entry to transfer")

    args: tuple[MediaEntry] = (media_entries[0],)

    job = await start_job(
        f'Transfer of file: {media_entries[0].name}', _transfer_file_job, session, args
    )

    return BaseResponse[Job](message='File transfer started', status_code=201, value=job)


def _transfer_file_job(
    media_entry: MediaEntry,
    job: Job,
    session: Session,
):
    current_full_path = Path(media_entry.absolute_path)

    remote_path = f'{secrets.media_system_root}/{current_full_path.name}'

    transfer_file(
        secrets.media_system_ip,
        secrets.media_system_username,
        secrets.media_system_password,
        media_entry.absolute_path,
        remote_path,
        media_entry,
        session,
        TransferDestination.MEDIA_SYSTEM,
    )


@router.get('/mediasystem', status_code=200)
async def current_transfers(
    session: Session = Depends(get_session),
) -> BaseResponse[list[TransferProgress]]:
    transfers = get_all_running_transfers()
    return BaseResponse[list[TransferProgress]](
        message='Transfer progresses found', status_code=200, value=transfers
    )
