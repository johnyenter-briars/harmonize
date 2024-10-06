from typing import TypedDict


class MagnetLinkSearchResult(TypedDict, total=False):
    magnet_link: str
    number_seeders: int | None
    number_leechers: int | None
    name: str | None
    number_downloads: int | None
    size: str | None
    date_posted: str | None
