from typing import Any, TypedDict


class DownloadVideoArguments(TypedDict):
    video_id: str
    video_metadata: Any


class DownloadPlaylistArguments(TypedDict):
    playlist_id: str
    playlist_metadata: Any
