import json
from typing import Any

import anyio
from fastapi import APIRouter
from youtubesearchpython import VideosSearch

router = APIRouter()


@router.get('/search/youtube/{search_keywords}')
async def search_youtube(search_keywords: str) -> str | dict[Any, Any]:
    videos_search = VideosSearch(search_keywords, limit=10)

    search_result = videos_search.result()

    async with await anyio.open_file(f'./cache/{search_keywords}.info.json', 'w') as f:
        await f.write(json.dumps(search_result))

    return search_result
