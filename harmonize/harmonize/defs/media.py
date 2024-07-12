from typing import TypedDict


class MediaElement(TypedDict):
    path: str

class MediaDownload(TypedDict):
    media_elements: list[MediaElement]
