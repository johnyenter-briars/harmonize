from harmonize.defs.response import BaseSchema


class Uptime(BaseSchema):
    seconds: int
    hours: int
    minutes: int


class Drive(BaseSchema):
    path: str
    space_used: float


class HealthStatus(BaseSchema):
    uptime: Uptime
    drives: list[Drive]
    vpn_connected: bool
    vpn_country: str
    cpu_usage_percent: float
    memory_usage_percent: float
    upload_speed_kb: float
    download_speed_kb: float
    audio_count: int
    video_count: int
    playlist_count: int
    season_count: int
