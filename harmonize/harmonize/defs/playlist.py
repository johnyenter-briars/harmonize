from harmonize.defs.response import BaseSchema


class Playlist(BaseSchema):
    files: list[str]
