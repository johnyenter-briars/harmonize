import shutil
from pathlib import Path

import harmonize.config
import harmonize.config.harmonizeconfig
from harmonize.const import VIDEO_ROOT

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG


def get_drive_free_space_shutil(drive: Path) -> int:
    return shutil.disk_usage(drive).fre


def get_drive_with_most_space() -> Path | None:
    drives = [Path(drive) for drive in config.drives]
    least_free_drive = max(drives, key=get_drive_free_space_shutil)
    return least_free_drive


def copy_file_to_mounted_folders(
    source_path: Path,
    chosen_drive: Path | None = None,
    new_name: str | None = None,
) -> Path | None:
    if chosen_drive is None:
        chosen_drive = get_drive_with_most_space()

    if chosen_drive is None:
        return None

    if new_name is None:
        destination_path = chosen_drive / VIDEO_ROOT / source_path.name
    else:
        destination_path = chosen_drive / VIDEO_ROOT / f'{new_name}{source_path.suffix}'

    shutil.copy2(source_path, destination_path)

    return destination_path


def remove_file(path: Path):
    if path.exists():
        if path.is_file():
            path.unlink()
        elif path.is_dir():
            shutil.rmtree(path)
