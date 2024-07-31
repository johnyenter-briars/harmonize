import json
from typing import Any

import anyio
from fastapi import APIRouter
from youtubesearchpython import VideosSearch

router = APIRouter(prefix='/api')


@router.get('/search/youtube/{search_keywords}')
async def search_youtube(search_keywords: str) -> str | dict[Any, Any]:
    videos_search = VideosSearch(search_keywords, limit=10)

    search_results = videos_search.result()

    for search_result in search_results['result']:  # type: ignore
        yt_id = search_result['id']  # type: ignore

        async with await anyio.open_file(f'./cache/youtube/{yt_id}.info.json', 'w') as f:
            await f.write(json.dumps(search_results))

    return search_results
