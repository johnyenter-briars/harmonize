import re

from bs4 import BeautifulSoup as bs

from harmonize.defs.magnetlink import MagnetLinkSearchResult
from harmonize.scrape import transform_torrent_data
from harmonize.util.fetch import get

_MAX_RESULTS_TO_RETURN = 100


async def _piratebay_search_page(query: str, page: int) -> list[MagnetLinkSearchResult]:
    query = query.replace('+', ' ')
    url = f'https://tpb.party/search/{query}/{page}/99/0'
    headers = {
        'User-Agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36'
    }

    (_, text) = await get(url, headers)

    seeders = []
    leechers = []
    names = []
    magnet_links = []
    downloads = []
    sizes = []
    date_posteds = []
    soup = bs(text, 'html.parser')

    a_tags = soup.find_all('a')  # for find href links
    font_tags = soup.find_all('font', {'class': 'detDesc'})
    tds = soup.find_all('td', {'align': 'right'})

    all_rows = soup.find_all('tr')
    for row in all_rows:
        cells = row.find_all('td')
        if len(cells) == 0:
            continue

        if len(cells) == 8:
            seeding = cells[5]
            leeching = cells[6]
            seeders.append(seeding.text)
            leechers.append(leeching.text)

        if len(cells) == 7:
            seeding = cells[5]
            leeching = cells[6]
            seeders.append(seeding.text)
            leechers.append(leeching.text)

        if len(cells) == 4:
            seeding = cells[2]
            leeching = cells[3]
            seeders.append(seeding.text)
            leechers.append(leeching.text)

    for link in a_tags:
        b: str = link.get('href')
        if b is not None and re.match('^magnet:', b):
            magnet_links.append(b)

        if b is not None and '/torrent/' in b:
            names.append(link.text)
            downloads.append(None)

    for tag in font_tags:
        text_split = tag.text.replace('\xa0', ' ').split(',')
        date_split = text_split[0].split(' ')
        date = f'{date_split[1]} {date_split[2]}'
        date_posteds.append(date)

        size_split = text_split[1].split(' ')
        size = f'{size_split[2]} {size_split[3]}'
        sizes.append(size)

    tds = soup.find_all('td')
    for td in tds:
        if 'GiB' not in td.text and 'MiB' not in td.text and 'KiB' not in td.text:
            continue

        sizes.append(td.text.replace('\xa0', ' '))

    date_pattern = r'^\d{2}-\d{2} \d{4}$'
    date_pattern2 = r'^\d{2}-\d{2} \d{2}:\d{2}$'
    date_pattern3 = r'^Y-day \d{2}:\d{2}$'

    for td in tds:
        if '-' not in td.text:
            continue

        replaced = td.text.replace('\xa0', ' ')

        if (
            re.match(date_pattern, replaced)
            or re.match(date_pattern2, replaced)
            or re.match(date_pattern3, replaced)
        ):
            date_posteds.append(replaced)

    return transform_torrent_data(
        magnet_links, seeders, leechers, names, downloads, sizes, date_posteds
    )


async def piratebay_search(
    query: str, all_pages: bool | None = None
) -> list[MagnetLinkSearchResult]:
    if all_pages is None or all_pages is False:
        return await _piratebay_search_page(query, 0)

    page = 1
    all_results = []
    while True:
        results = await _piratebay_search_page(query, page)
        all_results.extend(results)
        if len(results) == 0 or len(all_results) >= _MAX_RESULTS_TO_RETURN:
            return all_results
        page += 1
