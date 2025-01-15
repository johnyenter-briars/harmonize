import json

import anyio
import yt_dlp
from fastapi import APIRouter

from harmonize.const import YOUTUBE_PLAYLIST_SEARCH_METADATA, YOUTUBE_VIDEO_SEARCH_METADATA
from harmonize.defs.magnetlink import MagnetLinkSearchResult
from harmonize.defs.response import BaseResponse
from harmonize.defs.youtube import YoutubePlaylistSearchResult, YoutubeVideoSearchResult
from harmonize.scrape.piratebay import piratebay_search
from harmonize.scrape.xt1337 import t1337x_search

router = APIRouter(prefix='/api')

_MAX_RESULTS = 10


@router.get('/search/youtube/playlist/{search_keywords}')
async def search_youtube_playlist(
    search_keywords: str,
) -> BaseResponse[list[YoutubePlaylistSearchResult]]:
    ydl_opts = {
        'extract_flat': True,
        'force_generic_extractor': True,
        'playlistend': _MAX_RESULTS,
    }

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        result = ydl.extract_info(
            f'https://www.youtube.com/results?search_query={search_keywords}&sp=EgIQAw%253D%253D',
            download=False,
        )

    if result is None:
        return BaseResponse[list[YoutubePlaylistSearchResult]](
            message='Unable to property parse youtube search results', status_code=500, value=None
        )

    playlists = []
    for entry in result['entries']:
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

            playlist = YoutubePlaylistSearchResult(
                type='playlist',
                id=entry['id'],
                title=entry['title'],
                video_count=playlist_info.get('playlist_count', 'Unknown'),
                channel=playlist_info.get('uploader', 'Unknown'),
                thumbnails=entry['thumbnails'],
                link=entry['url'],
            )

            playlists.append(playlist)

    return BaseResponse[list[YoutubePlaylistSearchResult]](
        message='Success', status_code=200, value=playlists
    )


@router.get('/search/youtube/video/{search_keywords}')
async def search_youtube_video(
    search_keywords: str,
) -> BaseResponse[list[YoutubeVideoSearchResult]]:
    ydl_opts = {
        'extract_flat': True,
        'force_generic_extractor': True,
        'playlistend': _MAX_RESULTS,
    }

    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        result = ydl.extract_info(
            f'https://www.youtube.com/results?search_query={search_keywords}sp=EgIQAQ%253D%253D',
            download=False,
        )

    if result is None or 'entries' not in result:
        return BaseResponse[list[YoutubeVideoSearchResult]](
            message='Unable to properly parse YouTube search results', status_code=500, value=None
        )

    videos = []

    for entry in result['entries']:
        video_url = entry['url']
        video_info = ydl.extract_info(video_url, download=False)

        if video_info is None:
            return BaseResponse[list[YoutubeVideoSearchResult]](
                message='Unable to properly parse video metadata', status_code=500, value=None
            )

        metadata_file = YOUTUBE_VIDEO_SEARCH_METADATA / f'{entry["id"]}.search.info.json'

        async with await anyio.open_file(metadata_file, 'w') as f:
            await f.write(json.dumps(video_info))

        video_result = YoutubeVideoSearchResult(
            type='video',
            id=entry['id'],
            title=entry['title'],
            published_time=video_info.get('upload_date', 'Unknown'),
            duration=str(video_info.get('duration', 'Unknown')),
            view_count=video_info.get('view_count', 'Unknown'),
            thumbnails=entry['thumbnails'],
            rich_thumbnail=video_info.get('rich_thumbnails', None),
            channel=video_info.get('uploader', 'Unknown'),
            accessibility=None,
            link=entry['url'],
            shelf_title=None,
        )

        videos.append(video_result)

    return BaseResponse[list[YoutubeVideoSearchResult]](
        message='Success', status_code=200, value=videos
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
