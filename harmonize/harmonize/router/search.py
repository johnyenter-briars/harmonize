import json

import anyio
from fastapi import APIRouter
from youtubesearchpython import VideosSearch

from harmonize.const import YOUTUBE_SEARCH_METADATA
from harmonize.defs.magnetlinksearchresult import MagnetLinkSearchResult
from harmonize.defs.response import BaseResponse
from harmonize.scrape.piratebay import piratebay_search
from harmonize.scrape.xt1337 import t1337x_search

router = APIRouter(prefix='/api')


@router.get('/search/youtube/{search_keywords}')
async def search_youtube(search_keywords: str) -> BaseResponse[list[dict]]:
    videos_search = VideosSearch(search_keywords, limit=10)

    search_results = videos_search.result()

    if isinstance(search_results, str):
        return {
            'status_code': 500,
            'message': 'Unable to property parse youtube search results',
            'value': None,
        }

    for search_result in search_results['result']:
        yt_id = search_result['id']

        metadata_file = YOUTUBE_SEARCH_METADATA / f'{yt_id}.search.info.json'

        async with await anyio.open_file(metadata_file, 'w') as f:
            await f.write(json.dumps(search_result))

    return {'status_code': 200, 'message': 'success', 'value': search_results['result']}


@router.get('/search/piratebay/{search_keywords}')
async def search_piratebay(search_keywords: str) -> BaseResponse[list[MagnetLinkSearchResult]]:
    results = await piratebay_search(search_keywords)
    return {'status_code': 200, 'message': 'success', 'value': results}


@router.get('/search/xt1337/{search_keywords}')
async def search_xt1337(search_keywords: str) -> BaseResponse[list[MagnetLinkSearchResult]]:
    results = await t1337x_search(search_keywords)
    return {'status_code': 200, 'message': 'success', 'value': results}
