from harmonize.defs.response import BaseSchema


class Uptime(BaseSchema):
    seconds: int
    hours: int
    minutes: int


class Drive(BaseSchema):
    path: str
    space: int


class HealthStatus(BaseSchema):
    uptime: Uptime
    drives: list[Drive]
