from typing import TypedDict

from harmonize.defs.response import BaseSchema


class ApicData:
    data: bytes


class HarmonizeThumbnail(TypedDict):
    xl: str
    large: str
    small: str


class ArtworkMetadata(TypedDict):
    src: str
    sizes: str
    type: str
    name: str


class HarmonizeThumbnails(TypedDict):
    xl: str
    large: str
    small: str


class MediaMetadata(BaseSchema):
    title: str | None
    artist: str | None
    album: str | None
    artwork: HarmonizeThumbnail | None
