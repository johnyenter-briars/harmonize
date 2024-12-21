from enum import Enum

from harmonize.db.models import MediaEntry
from harmonize.defs.response import BaseSchema


class TransferDestination(Enum):
    MEDIA_SYSTEM = 0


class TransferProgress(BaseSchema):
    media_entry: MediaEntry
    destination: TransferDestination
    progress: float
