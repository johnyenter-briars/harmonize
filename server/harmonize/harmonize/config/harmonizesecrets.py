import json
import logging
from dataclasses import dataclass, field
from pathlib import Path

from harmonize.config import _kebab_to_snake
from harmonize.const import SECRETS_FILE


@dataclass
class HarmonizeSecrets:
    media_system_ip: str = field()
    media_system_username: str = field()
    media_system_password: str = field()
    media_system_root: str = field()
    harmonize_username: str = field()
    harmonize_password: str = field()

    def __init__(self, secrets_file: Path):
        with secrets_file.open('r') as f:
            config = json.load(f)

            snake_case_config = {_kebab_to_snake(k): v for k, v in config.items()}

            self.__dict__.update(snake_case_config)

    def log_config(self):
        logger = logging.getLogger('harmonize')

        for key, value in self.__dict__.items():
            logger.info('%s, %s', key, value)


HARMONIZE_SECRETS = HarmonizeSecrets(SECRETS_FILE)
