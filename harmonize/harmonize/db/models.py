import datetime
import uuid
from enum import Enum

from sqlmodel import Field, SQLModel

from harmonize.defs.response import BaseSchema


class JobStatus(Enum):
    SUCCEEDED = 0
    RUNNING = 1
    FAILED = 2
    CANCELED = 3


class Job(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    started_on: datetime.datetime
    description: str
    error_message: str | None
    status: JobStatus


class MediaElementSource(Enum):
    YOUTUBE = 0
    MAGNETLINK = 1


class MediaElementType(Enum):
    AUDIO = 0
    VIDEO = 1


class MediaEntry(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    name: str
    absolute_path: str
    source: MediaElementSource
    youtube_id: str | None
    type: MediaElementType
    date_added: datetime.datetime
    type: MediaElementType
