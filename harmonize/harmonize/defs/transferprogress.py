import datetime
import uuid
from enum import Enum

from harmonize.defs.response import BaseSchema


class TransferDestination(Enum):
    MEDIA_SYSTEM = 0


class TransferProgress(BaseSchema):
    media_entry_id: uuid.UUID
    name: str
    destination: TransferDestination
    progress: float
    start_time: datetime.datetime
