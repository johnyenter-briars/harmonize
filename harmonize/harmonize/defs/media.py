from pydantic import BaseModel

from harmonize.defs.response import BaseSchema


class MediaElement(BaseSchema):
    path: str


class MediaDownload(BaseSchema):
    media_elements: list[MediaElement]


class UpsertMediaEntryRequest(BaseModel):
    name: str
