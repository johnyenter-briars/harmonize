from harmonize.defs.response import BaseSchema


class Artist(BaseSchema):
    id: str
    name: str
    sortname: str
    disambiguation: str | None
    iso31661codes: list[str] | None


class ArtistCredit(BaseSchema):
    name: str
    artist: Artist


class Label(BaseSchema):
    id: str
    name: str


class LabelInfo(BaseSchema):
    label: Label
    catalognumber: str | None


class Media(BaseSchema):
    disccount: int
    trackcount: int
    format: str | None


class ReleaseEvent(BaseSchema):
    date: str
    area: Artist


class ReleaseGroup(BaseSchema):
    id: str
    typeid: str
    primarytypeid: str
    title: str
    primarytype: str


class Tag(BaseSchema):
    count: int
    name: str


class TextRepresentation(BaseSchema):
    language: str
    script: str


class Release(BaseSchema):
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


class MusicBrainzReleaseResponse(BaseSchema):
    created: str
    count: int
    offset: int
    releases: list[Release]


# Cover Art Archive Defs


class Thumbnails(BaseSchema):
    large: str
    small: str
    # Can't have python names start with numbers so can't type hint these properly
    # 250: str
    # 500: str
    # 1200: str


class Image(BaseSchema):
    approved: bool
    back: bool
    comment: str
    edit: int
    front: bool
    id: int
    image: str
    thumbnails: Thumbnails
    types: list[str]


class CoverArtArchiveResponse(BaseSchema):
    images: list[Image]
    release: str
