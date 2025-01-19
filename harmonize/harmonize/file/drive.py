import random
import shutil
from pathlib import Path

import harmonize.config
import harmonize.config.harmonizeconfig

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG


def _get_random_drive() -> Path:
    random_drive = random.choice(config.drives)  # noqa: S311

    return Path(random_drive)


def move_file_to_mounted_folders(source_path: Path) -> Path:
    chosen_drive = _get_random_drive()

    destination_path = chosen_drive / source_path

    shutil.copy2(source_path, destination_path)

    return destination_path


def remove_file(path: Path):
    if path.exists() and path.is_file():
        path.unlink()
