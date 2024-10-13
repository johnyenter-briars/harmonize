from harmonize.harmonize.defs.response import BaseSchema


class MediaElement(BaseSchema):
    path: str


class MediaDownload(BaseSchema):
    media_elements: list[MediaElement]
