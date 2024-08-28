from pathlib import Path

MUSIC_ROOT_LEGACY = Path('./music')
MEDIA_ROOT = Path('./media/audio/youtube')
MUSIC_ROOT = Path('./media/audio')
TMP_ALBUM_ART_DIR = Path('./cache/tmp/album_art')
YOUTUBE_METADATA = Path('./cache/youtube/metadata')
YOUTUBE_SEARCH_METADATA = Path('./cache/youtube/metadata/search/result')
CONFIG_FILE = Path('./config.json')

YOUTUBE_METADATA.mkdir(parents=True, exist_ok=True)
YOUTUBE_SEARCH_METADATA.mkdir(parents=True, exist_ok=True)
TMP_ALBUM_ART_DIR.mkdir(parents=True, exist_ok=True)
