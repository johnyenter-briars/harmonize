import json

import anyio
from fastapi import APIRouter
from youtubesearchpython import PlaylistsSearch, VideosSearch

from harmonize.const import YOUTUBE_PLAYLIST_SEARCH_METADATA, YOUTUBE_VIDEO_SEARCH_METADATA
from harmonize.defs.magnetlink import MagnetLinkSearchResult
from harmonize.defs.response import BaseResponse
from harmonize.defs.youtube import YoutubePlaylistSearchResult, YoutubeVideoSearchResult
from harmonize.scrape.piratebay import piratebay_search
from harmonize.scrape.xt1337 import t1337x_search

router = APIRouter(prefix='/api')


@router.get('/search/youtube/playlist/{search_keywords}')
async def search_youtube_playlist(
    search_keywords: str,
) -> BaseResponse[list[YoutubePlaylistSearchResult]]:
    playlist_search = PlaylistsSearch(search_keywords, limit=10)

    search_results = playlist_search.result()

    if isinstance(search_results, str):
        return BaseResponse[list[YoutubePlaylistSearchResult]](
            message='Unable to property parse youtube search results', status_code=500, value=None
        )

    for search_result in search_results['result']:
        playlist_id = search_result['id']

        metadata_file = YOUTUBE_PLAYLIST_SEARCH_METADATA / f'{playlist_id}.search.info.json'

        async with await anyio.open_file(metadata_file, 'w') as f:
            await f.write(json.dumps(search_result))

    results: list[YoutubePlaylistSearchResult] = search_results['result']

    return BaseResponse[list[YoutubePlaylistSearchResult]](
        message='Success', status_code=200, value=results
    )


@router.get('/search/youtube/video/{search_keywords}')
async def search_youtube_video(
    search_keywords: str,
) -> BaseResponse[list[YoutubeVideoSearchResult]]:
    videos_search = VideosSearch(search_keywords, limit=10)

    search_results = videos_search.result()

    if isinstance(search_results, str):
        return BaseResponse[list[YoutubeVideoSearchResult]](
            message='Unable to property parse youtube search results', status_code=500, value=None
        )

    for search_result in search_results['result']:
        yt_id = search_result['id']

        metadata_file = YOUTUBE_VIDEO_SEARCH_METADATA / f'{yt_id}.search.info.json'

        async with await anyio.open_file(metadata_file, 'w') as f:
            await f.write(json.dumps(search_result))

    results: list[YoutubeVideoSearchResult] = search_results['result']

    return BaseResponse[list[YoutubeVideoSearchResult]](
        message='Success', status_code=200, value=results
    )


@router.get('/search/piratebay/{search_keywords}')
async def search_piratebay(search_keywords: str) -> BaseResponse[list[MagnetLinkSearchResult]]:
    results = await piratebay_search(search_keywords)
    return BaseResponse[list[MagnetLinkSearchResult]](
        message='Success', status_code=200, value=results
    )


@router.get('/search/xt1337/{search_keywords}')
async def search_xt1337(search_keywords: str) -> BaseResponse[list[MagnetLinkSearchResult]]:
    results = await t1337x_search(search_keywords)
    return BaseResponse[list[MagnetLinkSearchResult]](
        message='Success', status_code=200, value=results
    )
