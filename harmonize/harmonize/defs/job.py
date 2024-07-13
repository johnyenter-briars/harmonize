import uuid
from datetime import datetime
from enum import Enum
from typing import TypedDict


class Status(Enum):
    SUCCEEDED = 0
    RUNNING = 1
    FAILED = 2

class Job(TypedDict):
    id: uuid.UUID
    started_on: datetime
    description: str
    status: Status

