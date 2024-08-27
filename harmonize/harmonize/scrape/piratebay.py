import requests
from bs4 import BeautifulSoup as bs

from harmonize.defs.magnetlinksearchresult import MagnetLinkSearchResult


def piratebay_search(query) -> list[MagnetLinkSearchResult]:
    query = query.replace('+', ' ')
    url = f'https://tpb.party/search/{query}/1/99/0'
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.2840.71 Safari/539.36'
    }

    source = requests.get(url, headers=headers)

    seeders = []
    leechers = []
    names = []
    magnet_links = []
    downloads = []
    sizes = []
    date_posteds = []
    soup = bs(source.text, 'html.parser')

    a_tags = soup.find_all('a')  # for find href links
    font_tags = soup.find_all('font', {'class': 'detDesc'})
    tds = soup.find_all('td', {'align': 'right'})

    paired = [
        (item_1.text, item_2.text) for item_1, item_2 in zip(tds[::2], tds[1::2], strict=False)
    ]

    for seeder, leecher in paired:
        seeders.append(seeder)
        leechers.append(leecher)

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

    return [
        MagnetLinkSearchResult(
            magnet_link, number_seeders, number_leechers, name, download, size, date_posted
        )
        for magnet_link, number_seeders, number_leechers, name, download, size, date_posted in zip(
            magnet_links, seeders, leechers, names, downloads, sizes, date_posteds, strict=False
        )
    ]
