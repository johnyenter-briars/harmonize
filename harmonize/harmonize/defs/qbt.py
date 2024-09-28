from pydantic import BaseModel


class AddTorrentRequest(BaseModel):
    magnet_links: list[str]


class PauseTorrentsRequest(BaseModel):
    hashes: list[str]


class ResumeTorrentsRequest(BaseModel):
    hashes: list[str]


class DeleteTorrentsRequest(BaseModel):
    hashes: list[str]


class TorrentData(BaseModel):
    added_on: int
    amount_left: int
    auto_tmm: bool
    availability: float
    category: str
    completed: int
    completion_on: int
    content_path: str
    dl_limit: int
    dlspeed: int
    download_path: str | None
    downloaded: int
    downloaded_session: int
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
    max_ratio: int
    max_seeding_time: int
    name: str
    num_complete: int
    num_incomplete: int
    num_leechs: int
    num_seeds: int
    priority: int
    progress: float
    ratio: int
    ratio_limit: int
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
