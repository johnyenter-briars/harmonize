from pathlib import Path
from typing import Final

MUSIC_ROOT_LEGACY = Path('./music')
MEDIA_ROOT = Path('./media/audio/youtube')
AUDIO_ROOT = Path('./media/audio')
VIDEO_ROOT = Path('./media/video')
TMP_ALBUM_ART_DIR = Path('./cache/tmp/album_art')
YOUTUBE_METADATA = Path('./cache/youtube/metadata')
YOUTUBE_VIDEO_SEARCH_METADATA = Path('./cache/youtube/metadata/search/result/video')
YOUTUBE_PLAYLIST_SEARCH_METADATA = Path('./cache/youtube/metadata/search/result/playlist')
YOUTUBE_VIDEO_YTDL_METADATA = Path('./cache/youtube/metadata/ytdl/video')
YOUTUBE_PLAYLIST_YTDL_METADATA = Path('./cache/youtube/metadata/ytdl/playlist')
CONFIG_FILE = Path('./config.json')
SECRETS_FILE = Path('./secrets.json')

COVERART_ARCHIVE_ROOT: Final = 'http://coverartarchive.org/release'

MUSTICBRAINZ_RELEASE_ROOT: Final = (
    'https://musicbrainz.org/ws/2/release/?query={query_parameters}&fmt=json'
)

YOUTUBE_METADATA.mkdir(parents=True, exist_ok=True)
YOUTUBE_VIDEO_SEARCH_METADATA.mkdir(parents=True, exist_ok=True)
YOUTUBE_PLAYLIST_SEARCH_METADATA.mkdir(parents=True, exist_ok=True)
TMP_ALBUM_ART_DIR.mkdir(parents=True, exist_ok=True)
