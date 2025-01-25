import shutil
from pathlib import Path

import harmonize.config
import harmonize.config.harmonizeconfig
from harmonize.const import VIDEO_ROOT

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG


def get_folder_size_bytes(folder: Path) -> int:
    total_size = 0
    for entry in folder.iterdir():
        if entry.is_file():
            total_size += entry.stat().st_size
        elif entry.is_dir():
            total_size += get_folder_size_bytes(entry)
    return total_size


def get_drive_with_least_space() -> Path | None:
    drives = [Path(drive) for drive in config.drives]
    folder_sizes = {drive: get_folder_size_bytes(drive) for drive in drives}

    least_space_drive = None
    least_space = float('inf')
    for drive, size in folder_sizes.items():
        if size < least_space:
            least_space_drive = drive
            least_space = size

    return least_space_drive


def move_file_to_mounted_folders(
    source_path: Path, chosen_drive: Path | None = None
) -> Path | None:
    if chosen_drive is None:
        chosen_drive = get_drive_with_least_space()

    if chosen_drive is None:
        return None

    destination_path = chosen_drive / VIDEO_ROOT / source_path.name

    shutil.copy2(source_path, destination_path)

    return destination_path


def remove_file(path: Path):
    if path.exists():
        if path.is_file():
            path.unlink()
        elif path.is_dir():
            shutil.rmtree(path)
