from harmonize.defs.magnetlink import MagnetLinkSearchResult


def transform_torrent_data(
    magnet_links: list[str],
    seeders: list[str],
    leechers: list[str],
    names: list[str],
    downloads: list[str],
    sizes: list[str],
    date_posteds: list[str],
) -> list[MagnetLinkSearchResult]:
    return [
        MagnetLinkSearchResult(
            magnet_link=magnet_link,
            number_seeders=number_seeders,
            number_leechers=number_leechers,
            name=name,
            number_downloads=number_downloads,
            size=size,
            date_posted=date_posted,
        )
        for magnet_link, number_seeders, number_leechers, name, number_downloads, size, date_posted in zip(
            magnet_links, seeders, leechers, names, downloads, sizes, date_posteds, strict=False
        )
    ]

