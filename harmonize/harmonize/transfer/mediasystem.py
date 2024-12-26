import logging
import uuid

import paramiko
from sqlmodel import Session

from harmonize.db.models import MediaEntry
from harmonize.defs.transferprogress import TransferDestination, TransferProgress

logger = logging.getLogger('harmonize')

_progress_dict: dict[tuple[uuid.UUID, TransferDestination], TransferProgress] = {}


def _generate_progress_callback(
    media_entry: MediaEntry, session: Session, transfer_destination: TransferDestination
):
    media_entry = session.merge(media_entry)

    key = (media_entry.id, transfer_destination)

    _progress_dict[key] = TransferProgress(
        media_entry=media_entry, destination=TransferDestination.MEDIA_SYSTEM, progress=0
    )

    def _progress_callback(transferred, total):
        progress_percentage = (transferred / total) * 100

        _progress_dict[key].progress = round(progress_percentage, 2)

    return _progress_callback


def transfer_file(
    ip: str,
    username: str,
    password: str,
    local_path: str,
    remote_path: str,
    media_entry: MediaEntry,
    session: Session,
    transfer_destination: TransferDestination,
) -> None:
    media_entry = session.merge(media_entry)

    ssh_client = paramiko.SSHClient()
    ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    ssh_client.connect(hostname=ip, username=username, password=password)

    sftp = ssh_client.open_sftp()

    key = (media_entry.id, transfer_destination)

    if key in _progress_dict:
        progress_entry = _progress_dict[key]
        if progress_entry.progress < 100:
            logger.info(
                f'key: {key} already in progress dict and current progress: {progress_entry.progress}'
            )
            return

    sftp.put(
        local_path,
        remote_path,
        callback=_generate_progress_callback(media_entry, session, transfer_destination),
    )

    logger.info(f'File transferred successfully to {remote_path}')  # noqa: G004

    sftp.close()
    ssh_client.close()


def get_running_transfer(media_entry: MediaEntry) -> TransferProgress | None:
    if media_entry.id not in _progress_dict:
        return None

    return _progress_dict[media_entry.id]


def get_all_running_transfers() -> list[TransferProgress]:
    foo = _progress_dict.values()
    return list(foo)
