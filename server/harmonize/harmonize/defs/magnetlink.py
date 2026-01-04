from harmonize.defs.response import BaseSchema


class MagnetLinkSearchResult(BaseSchema):
    magnet_link: str
    number_seeders: int | None
    number_leechers: int | None
    name: str | None
    number_downloads: int | None
    size: str | None
    date_posted: str | None
