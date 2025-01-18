import datetime
import uuid
from enum import Enum

from sqlmodel import Field, Relationship, SQLModel

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


class MediaEntryType(Enum):
    AUDIO = 0
    VIDEO = 1
    SUBTITLE = 2


class MediaEntryPlaylistLink(SQLModel, table=True):
    playlist_id: uuid.UUID = Field(foreign_key='playlist.id', primary_key=True)
    media_entry_id: uuid.UUID = Field(foreign_key='mediaentry.id', primary_key=True)


class Playlist(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    name: str
    date_created: datetime.datetime = Field(default_factory=datetime.datetime.utcnow)

    media_entries: list['MediaEntry'] = Relationship(
        back_populates='playlists', link_model=MediaEntryPlaylistLink
    )


class MediaEntry(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    name: str
    absolute_path: str
    source: MediaElementSource
    youtube_id: str | None
    magnet_link: str | None
    type: MediaEntryType
    date_added: datetime.datetime
    cover_art_absolute_path: str | None
    thumbnail_art_absolute_path: str | None
    playlists: list['Playlist'] = Relationship(
        back_populates='media_entries', link_model=MediaEntryPlaylistLink
    )
