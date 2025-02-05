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
    key: str
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


class VideoType(Enum):
    MOVIE = 0
    EPISODE = 1


class AudioType(Enum):
    SONG = 0
    AUDIOBOOK = 1


class MediaEntryPlaylistLink(SQLModel, table=True):
    playlist_id: uuid.UUID = Field(foreign_key='playlist.id', primary_key=True)
    media_entry_id: uuid.UUID = Field(foreign_key='mediaentry.id', primary_key=True)


class Playlist(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    name: str
    date_added: datetime.datetime = Field(default_factory=datetime.datetime.utcnow)

    media_entries: list['MediaEntry'] = Relationship(
        back_populates='playlists', link_model=MediaEntryPlaylistLink
    )


class Season(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    name: str
    date_added: datetime.datetime = Field(default_factory=datetime.datetime.utcnow)
    media_entries: list['MediaEntry'] = Relationship(back_populates='season')


class MediaEntry(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    name: str
    absolute_path: str
    source: MediaElementSource
    youtube_id: str | None
    magnet_link: str | None
    type: MediaEntryType
    video_type: VideoType | None
    audio_type: AudioType | None
    date_added: datetime.datetime
    cover_art_absolute_path: str | None
    thumbnail_art_absolute_path: str | None
    transferred: bool  # TODO: transferred to where?

    playlists: list['Playlist'] = Relationship(
        back_populates='media_entries', link_model=MediaEntryPlaylistLink
    )

    season_id: uuid.UUID | None = Field(foreign_key='season.id', nullable=True)
    season: 'Season' = Relationship(back_populates='media_entries')

    # For subtitles
    parent_media_entry_id: uuid.UUID | None = Field(foreign_key='mediaentry.id', nullable=True)
    parent_media_entry: 'MediaEntry' = Relationship(
        sa_relationship_kwargs={'remote_side': 'MediaEntry.id'}
    )


class QbtDownloadTagInfo(BaseSchema, SQLModel, table=True):
    id: uuid.UUID = Field(default_factory=uuid.uuid4, primary_key=True)
    magnet_link: str
    type: MediaEntryType
    video_type: VideoType | None
    audio_type: AudioType | None
    create_season: bool | None
