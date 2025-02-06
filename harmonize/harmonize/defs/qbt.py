from pydantic import BaseModel, Field

from harmonize.db.models import AudioType, MediaEntryType, VideoType


class AddDownloadRequest(BaseModel):
    magnet_links: list[str] = Field(
        alias='magnetLinks'
    )  # TODO: this should really only be one. We currently treat it as if it's a list of 1
    name: str
    type: MediaEntryType = Field(alias='type')
    video_type: VideoType | None = Field(alias='videoType')
    audio_type: AudioType | None = Field(alias='audioType')
    create_season: bool | None = Field(alias='createSeason')


class PauseDownloadsRequest(BaseModel):
    hashes: list[str]


class ResumeDownloadsRequest(BaseModel):
    hashes: list[str]


class DeleteDownloadsRequest(BaseModel):
    hashes: list[str]


class QbtDownloadData(BaseModel):
    added_on: int
    amount_left: int
    auto_tmm: bool
    availability: float
    category: str
    completed: float
    completion_on: int
    content_path: str
    dl_limit: int
    dlspeed: int
    download_path: str | None
    downloaded: float
    downloaded_session: float
    eta: int
    f_l_piece_prio: bool
    force_start: bool
    hash: str
    inactive_seeding_time_limit: int
    infohash_v1: str
    infohash_v2: str | None
    last_activity: int
    magnet_uri: str
    max_inactive_seeding_time: int
    max_ratio: float
    max_seeding_time: int
    name: str
    num_complete: int
    num_incomplete: int
    num_leechs: int
    num_seeds: int
    priority: int
    progress: float
    ratio: float
    ratio_limit: float
    save_path: str
    seeding_time: int
    seeding_time_limit: int
    seen_complete: int
    seq_dl: bool
    size: int
    state: str
    super_seeding: bool
    tags: str
    time_active: int
    total_size: int
    tracker: str
    trackers_count: int
    up_limit: int
    uploaded: int
    uploaded_session: int
    upspeed: int
