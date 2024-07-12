
from fastapi import APIRouter
from youtubesearchpython import VideosSearch
import json

router = APIRouter()


@router.get("/search/youtube/{search_keywords}")
async def search_youtube(search_keywords: str) -> dict:
    videosSearch = VideosSearch(search_keywords, limit=10)

    search_result: dict = videosSearch.result()  # type: ignore

    with open(f"./cache/{search_keywords}.info.json", "w") as f:
        f.write(json.dumps(search_result))

    return search_result
