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
