from typing import TypedDict


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


class MediaMetadata(TypedDict):
    title: str | None
    artist: str | None
    album: str | None
    artwork: HarmonizeThumbnail | None
