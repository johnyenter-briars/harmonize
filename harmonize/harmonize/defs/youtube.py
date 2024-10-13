from typing import Any

from harmonize.defs.response import BaseSchema


class ViewCount(BaseSchema):
    text: str | None
    short: str | None


class Thumbnail(BaseSchema):
    url: str | None
    width: int | None
    height: int | None


class RichThumbnail(BaseSchema):
    url: str | None
    width: int | None
    height: int | None


class DescriptionSnippet(BaseSchema):
    text: str | None
    # bold: bool | None


class ChannelThumbnail(BaseSchema):
    url: str | None
    width: int | None
    height: int | None


class Channel(BaseSchema):
    name: str | None
    id: str | None
    thumbnails: list[ChannelThumbnail] | None
    link: str | None


class Accessibility(BaseSchema):
    title: str | None
    duration: str | None


class YoutubeVideoSearchResult(BaseSchema):
    type: str
    id: str
    title: str
    published_time: str | None
    duration: str | None
    view_count: ViewCount | None
    thumbnails: list[Thumbnail]
    rich_thumbnail: RichThumbnail | None
    description_snippet: list[DescriptionSnippet]
    channel: Channel
    accessibility: Accessibility | None
    link: str | None
    shelf_title: str | None


class PlaylistChannel(BaseSchema):
    name: str | None
    id: str | None
    link: str | None


class PlaylistThumbnail(BaseSchema):
    url: str | None
    width: int | None
    height: int | None


class YoutubePlaylistSearchResult(BaseSchema):
    type: str
    id: str
    title: str
    video_count: str
    channel: PlaylistChannel | None
    thumbnails: list[PlaylistThumbnail] | None
    link: str | None


class DownloadVideoArguments(BaseSchema):
    video_id: str
    video_metadata: Any


class DownloadPlaylistArguments(BaseSchema):
    playlist_id: str
    playlist_metadata: Any
