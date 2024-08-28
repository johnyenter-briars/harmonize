import json
from typing import Any

import anyio
from fastapi import APIRouter
from youtubesearchpython import VideosSearch

from harmonize.const import YOUTUBE_SEARCH_METADATA
from harmonize.defs.magnetlinksearchresult import MagnetLinkSearchResult
from harmonize.defs.response import BaseResponse
from harmonize.scrape.piratebay import piratebay_search

router = APIRouter(prefix='/api')


@router.get('/search/youtube/{search_keywords}')
async def search_youtube(search_keywords: str) -> str | dict[Any, Any]:
    videos_search = VideosSearch(search_keywords, limit=10)

    search_results = videos_search.result()

    for search_result in search_results['result']:  # type: ignore
        yt_id = search_result['id']  # type: ignore

        metadata_file = YOUTUBE_SEARCH_METADATA / f'{yt_id}.search.info.json'

        async with await anyio.open_file(metadata_file, 'w') as f:
            await f.write(json.dumps(search_result))

    return search_results


@router.get('/search/piratebay/{search_keywords}')
async def search_piratebay(search_keywords: str) -> BaseResponse[list[MagnetLinkSearchResult]]:
    results = await piratebay_search(search_keywords)
    return {'status_code': 200, 'message': 'success', 'value': results}
