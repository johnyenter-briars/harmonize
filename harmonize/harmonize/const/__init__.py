from pathlib import Path
from typing import Final

MUSIC_ROOT_LEGACY = Path('./music')
MEDIA_ROOT = Path('./media/audio/youtube')
AUDIO_ROOT = Path('./media/audio')
VIDEO_ROOT = Path('./media/video')
VIDEO_IMAGE_ROOT = Path('./media/video/img')
AUDIO_IMAGE_ROOT = Path('./media/audio/img')
VIDEO_IMAGE_COVER_ROOT = Path('./media/video/img/cover')
AUDIO_IMAGE_COVER_ROOT = Path('./media/audio/img/cover')
VIDEO_IMAGE_THUMBNAIL_ROOT = Path('./media/video/img/thumbnail')
AUDIO_IMAGE_THUMBNAIL_ROOT = Path('./media/audio/img/thumbnail')
TMP_ALBUM_ART_DIR = Path('./cache/tmp/album_art')
YOUTUBE_METADATA = Path('./cache/youtube/metadata')
YOUTUBE_VIDEO_SEARCH_METADATA = Path('./cache/youtube/metadata/search/result/video')
YOUTUBE_PLAYLIST_SEARCH_METADATA = Path('./cache/youtube/metadata/search/result/playlist')
YOUTUBE_VIDEO_YTDL_METADATA = Path('./cache/youtube/metadata/ytdl/video')
YOUTUBE_PLAYLIST_YTDL_METADATA = Path('./cache/youtube/metadata/ytdl/playlist')
CONFIG_FILE = Path('./config.json')
SECRETS_FILE = Path('./secrets.json')
SUPPORTED_EXTENSIONS = {'.srt', '.mkv', '.mp4'}
VIDEO_EXTENSIONS = {'.mkv', '.mp4'}

COVERART_ARCHIVE_ROOT: Final = 'http://coverartarchive.org/release'

MUSTICBRAINZ_RELEASE_ROOT: Final = (
    'https://musicbrainz.org/ws/2/release/?query={query_parameters}&fmt=json'
)

YOUTUBE_METADATA.mkdir(parents=True, exist_ok=True)
YOUTUBE_VIDEO_SEARCH_METADATA.mkdir(parents=True, exist_ok=True)
YOUTUBE_PLAYLIST_SEARCH_METADATA.mkdir(parents=True, exist_ok=True)
TMP_ALBUM_ART_DIR.mkdir(parents=True, exist_ok=True)
