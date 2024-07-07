from typing import TypedDict


class ArtworkMetadata(TypedDict):
    src: str
    sizes: str
    type: str
    name: str


class MediaMetadata(TypedDict):
    title: str
    artist: str
    album: str
    artwork: list[ArtworkMetadata]
