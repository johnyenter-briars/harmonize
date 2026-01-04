from typing import TypedDict


class Artist(TypedDict):
    id: str
    name: str
    sortname: str
    disambiguation: str | None
    iso31661codes: list[str] | None


class ArtistCredit(TypedDict):
    name: str
    artist: Artist


class Label(TypedDict):
    id: str
    name: str


class LabelInfo(TypedDict):
    label: Label
    catalognumber: str | None


class Media(TypedDict):
    disccount: int
    trackcount: int
    format: str | None


class ReleaseEvent(TypedDict):
    date: str
    area: Artist


class ReleaseGroup(TypedDict):
    id: str
    typeid: str
    primarytypeid: str
    title: str
    primarytype: str


class Tag(TypedDict):
    count: int
    name: str


class TextRepresentation(TypedDict):
    language: str
    script: str


class Release(TypedDict):
    id: str
    score: int
    statusid: str
    count: int
    title: str
    status: str
    artistcredit: list[ArtistCredit]
    releasegroup: ReleaseGroup
    date: str
    country: str
    releaseevents: list[ReleaseEvent]
    trackcount: int
    media: list[Media]
    packagingid: str | None
    packaging: str | None
    textrepresentation: TextRepresentation | None
    barcode: str | None
    labelinfo: list[LabelInfo] | None
    tags: list[Tag] | None
    asin: str | None


class MusicBrainzReleaseResponse(TypedDict):
    created: str
    count: int
    offset: int
    releases: list[Release]


# Cover Art Archive Defs


class Thumbnails(TypedDict):
    large: str
    small: str
    # Can't have python names start with numbers so can't type hint these properly
    # 250: str
    # 500: str
    # 1200: str


class Image(TypedDict):
    approved: bool
    back: bool
    comment: str
    edit: int
    front: bool
    id: int
    image: str
    thumbnails: Thumbnails
    types: list[str]


class CoverArtArchiveResponse(TypedDict):
    images: list[Image]
    release: str
