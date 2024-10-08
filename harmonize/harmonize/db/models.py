import datetime
import uuid
from enum import Enum

from sqlmodel import Field, SQLModel


class Status(Enum):
    SUCCEEDED = 0
    RUNNING = 1
    FAILED = 2
    CANCELED = 3


class Job(SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    started_on: datetime.datetime
    description: str
    error_message: str | None
    status: Status
