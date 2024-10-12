from typing import Any, TypedDict


class ViewCount(TypedDict):
    text: str
    short: str


class Thumbnail(TypedDict):
    url: str
    width: int
    height: int


class RichThumbnail(TypedDict):
    url: str
    width: int
    height: int


class DescriptionSnippet(TypedDict):
    text: str | None
    # bold: bool | None


class ChannelThumbnail(TypedDict):
    url: str
    width: int
    height: int


class Channel(TypedDict):
    name: str
    id: str
    thumbnails: list[ChannelThumbnail]
    link: str


class Accessibility(TypedDict):
    title: str
    duration: str


class YoutubeVideoSearchResult(TypedDict):
    type: str
    id: str
    title: str
    publishedTime: str
    duration: str
    viewCount: ViewCount
    thumbnails: list[Thumbnail]
    richThumbnail: RichThumbnail | None
    descriptionSnippet: list[DescriptionSnippet]
    channel: Channel
    accessibility: Accessibility
    link: str
    shelfTitle: str | None


class PlaylistChannel(TypedDict):
    name: str
    id: str
    link: str


class PlaylistThumbnail(TypedDict):
    url: str
    width: int
    height: int


class YoutubePlaylistSearchResult(TypedDict):
    type: str
    id: str
    title: str
    videoCount: str
    channel: PlaylistChannel
    thumbnails: list[PlaylistThumbnail]
    link: str


class DownloadVideoArguments(TypedDict):
    video_id: str
    video_metadata: Any


class DownloadPlaylistArguments(TypedDict):
    playlist_id: str
    playlist_metadata: Any
