from harmonize.defs.response import BaseSchema


class ApicData:
    data: bytes


class HarmonizeThumbnail(BaseSchema):
    xl: str
    large: str
    small: str


class ArtworkMetadata(BaseSchema):
    src: str
    sizes: str
    type: str
    name: str


class HarmonizeThumbnails(BaseSchema):
    xl: str
    large: str
    small: str


class MediaMetadata(BaseSchema):
    title: str | None
    artist: str | None
    album: str | None
    artwork: HarmonizeThumbnail | None
