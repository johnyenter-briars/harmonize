from typing import List

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
    # Determine the minimum length of the required non-optional fields
    min_length = min(
        len(magnet_links),
        len(names),
        len(downloads),
        len(sizes),
        len(date_posteds),
    )

    # If seeders or leechers are empty, fill them with "0"
    seeders = ['0'] * min_length if len(seeders) == 0 else seeders[:min_length]

    leechers = ['0'] * min_length if len(leechers) == 0 else leechers[:min_length]

    return [
        MagnetLinkSearchResult(
            magnet_link=magnet_links[i],
            number_seeders=int(seeders[i]),
            number_leechers=int(leechers[i]),
            name=names[i],
            number_downloads=None if downloads[i] is None else int(downloads[i]),
            size=sizes[i],
            date_posted=date_posteds[i],
        )
        for i in range(min_length)
    ]
