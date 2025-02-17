import logging
import uuid
from operator import iand
from pathlib import Path

from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy import select
from sqlmodel import Session, select

import harmonize.config
import harmonize.config.harmonizeconfig
import harmonize.config.harmonizesecrets
from harmonize.db.database import get_new_session, get_session
from harmonize.db.models import Job, MediaEntry, MediaEntryType
from harmonize.defs.response import BaseResponse
from harmonize.defs.transferprogress import TransferDestination, TransferProgress
from harmonize.job.callback import start_job
from harmonize.transfer.mediasystem import (
    get_all_running_transfers,
    remove_remote_file,
    transfer_file,
)

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG  # type: ignore
secrets = harmonize.config.harmonizesecrets.HARMONIZE_SECRETS


logger = logging.getLogger('harmonize')
router = APIRouter(prefix='/api/transfer')


@router.post('/mediasystem/{media_entry_id}', status_code=201)
async def transfer_file_mediasystem(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    media_entry = session.get(MediaEntry, media_entry_id)

    if media_entry is None:
        raise HTTPException(status_code=404, detail="Can't find media entry to transfer")

    args: tuple[uuid.UUID] = (media_entry.id,)

    job = await start_job(
        f'TSF-{media_entry.name}',
        f'Transfer of file: {media_entry.name}',
        _transfer_file_job,
        session,
        args,
    )

    return BaseResponse[Job](message='File transfer started', status_code=201, value=job)


def _transfer_file_job(
    media_entry_id: uuid.UUID,
    _: Job,
    __: Session,
):
    process_scoped_session = get_new_session()

    media_entry_proc = process_scoped_session.get(MediaEntry, media_entry_id)

    if media_entry_proc is None:
        return

    subs = list(
        process_scoped_session.exec(
            select(MediaEntry).where(
                iand(
                    MediaEntry.type == MediaEntryType.SUBTITLE,
                    MediaEntry.parent_media_entry_id == media_entry_id,
                )
            )
        )
    )

    current_full_path = Path(media_entry_proc.absolute_path)

    remote_path = f'{secrets.media_system_root}/{current_full_path.name}'

    transfer_file(
        secrets.media_system_ip,
        secrets.media_system_username,
        secrets.media_system_password,
        media_entry_proc.absolute_path,
        remote_path,
        media_entry_proc.name,
        media_entry_proc.id,
        TransferDestination.MEDIA_SYSTEM,
    )

    for sub in subs:
        current_full_path = Path(sub.absolute_path)

        remote_path = f'{secrets.media_system_root}/{current_full_path.name}'

        transfer_file(
            secrets.media_system_ip,
            secrets.media_system_username,
            secrets.media_system_password,
            sub.absolute_path,
            remote_path,
            sub.name,
            sub.id,
            TransferDestination.MEDIA_SYSTEM,
        )

        sub.transferred = True

    media_entry_proc.transferred = True

    process_scoped_session.commit()


@router.get('/mediasystem', status_code=200)
async def current_transfers(
    session: Session = Depends(get_session),
) -> BaseResponse[list[TransferProgress]]:
    transfers = get_all_running_transfers()
    return BaseResponse[list[TransferProgress]](
        message='Transfer progresses found', status_code=200, value=transfers
    )


@router.post('/mediasystem/{media_entry_id}/untransfer', status_code=201)
async def untransfer_file_mediasystem(
    media_entry_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    media_entry = session.get(MediaEntry, media_entry_id)

    if media_entry is None:
        raise HTTPException(status_code=404, detail="Can't find media entry to untransfer")

    args: tuple[uuid.UUID] = (media_entry.id,)

    job = await start_job(
        f'UTSF-{media_entry.name}',
        f'Untransfer of file: {media_entry.name}',
        _untransfer_file_job,
        session,
        args,
    )

    return BaseResponse[Job](message='File transfer started', status_code=201, value=job)


def _untransfer_file_job(
    media_entry_id: uuid.UUID,
    _: Job,
    __: Session,
):
    process_scoped_session = get_new_session()

    media_entry_proc = process_scoped_session.get(MediaEntry, media_entry_id)

    if media_entry_proc is None:
        return

    subs = list(
        process_scoped_session.exec(
            select(MediaEntry).where(
                iand(
                    MediaEntry.type == MediaEntryType.SUBTITLE,
                    MediaEntry.parent_media_entry_id == media_entry_id,
                )
            )
        )
    )

    current_full_path = Path(media_entry_proc.absolute_path)

    remote_path = f'{secrets.media_system_root}/{current_full_path.name}'

    remove_remote_file(
        secrets.media_system_ip,
        secrets.media_system_username,
        secrets.media_system_password,
        remote_path,
    )

    for sub in subs:
        current_full_path = Path(sub.absolute_path)

        remote_path = f'{secrets.media_system_root}/{current_full_path.name}'

        remove_remote_file(
            secrets.media_system_ip,
            secrets.media_system_username,
            secrets.media_system_password,
            remote_path,
        )

        sub.transferred = False

    media_entry_proc.transferred = False

    process_scoped_session.commit()
