from typing import TypedDict


class ApicData:
    data: bytes


class ArtworkMetadata(TypedDict):
    src: str
    sizes: str
    type: str


class HarmonizeThumbnails(TypedDict):
    xl: str
    large: str
    small: str


class MediaMetadata(TypedDict):
    title: str
    artist: str
    album: str
    artwork: HarmonizeThumbnails
