from enum import Enum
from typing import TypedDict
import uuid
from datetime import datetime


class Job(TypedDict):
    id: uuid.UUID
    started_on: datetime
    description: str
    status: int


class Color(Enum):
    RED = 1
    GREEN = 2
    BLUE = 3