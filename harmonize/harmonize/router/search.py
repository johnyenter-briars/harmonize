import json

import anyio
import yt_dlp
from fastapi import APIRouter
from youtubesearchpython import VideosSearch

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
    max_results = 10
    search_query = f'ytsearch{max_results}: {search_keywords}'
    ydl_opts = {
        'extract_flat': True,
        'force_generic_extractor': True,
    }

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        result = ydl.extract_info(
            'https://www.youtube.com/results?search_query=Ice Death Planets Lungs Mushrooms And Lava&sp=EgIQAw%253D%253D',
            download=False,
        )

    if result is None:
        return BaseResponse[list[YoutubePlaylistSearchResult]](
            message='Unable to property parse youtube search results', status_code=500, value=None
        )

    first_10_playlists = result['entries'][:10]
    playlists = []
    for entry in first_10_playlists:
        if 'playlist' in entry.get('url', ''):
            playlist_url = entry['url']
            playlist_info = ydl.extract_info(playlist_url, download=False)

            if playlist_info is None:
                return BaseResponse[list[YoutubePlaylistSearchResult]](
                    message='Unable to property parse youtube search results',
                    status_code=500,
                    value=None,
                )

            metadata_file = YOUTUBE_PLAYLIST_SEARCH_METADATA / f'{entry['id']}.search.info.json'

            async with await anyio.open_file(metadata_file, 'w') as f:
                await f.write(json.dumps(playlist_info))

            foo = YoutubePlaylistSearchResult(
                type='playlist',
                id=entry['id'],
                title=entry['title'],
                video_count=str(playlist_info.get('playlist_count', 'Unknown')),
                channel=playlist_info.get('uploader', 'Unknown'),
                thumbnails=entry['thumbnails'],
                link=entry['url'],
            )

            playlists.append(foo)

    results: list[YoutubePlaylistSearchResult] = playlists

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
