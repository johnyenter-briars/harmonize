import json
import logging
from dataclasses import dataclass, field
from pathlib import Path

from harmonize.const import CONFIG_FILE


def _kebab_to_snake(name: str) -> str:
    return name.replace('-', '_')


@dataclass
class HarmonizeConfig:
    media_root: str = field()
    qbt_domain_name: str = field()
    qbt_port: int = field()
    qbt_version: str = field()
    run_qbt: bool = field()

    def __init__(self, config_file: Path):
        with config_file.open('r') as f:
            config = json.load(f)

            snake_case_config = {_kebab_to_snake(k): v for k, v in config.items()}

            self.__dict__.update(snake_case_config)

    def log_config(self):
        logger = logging.getLogger('harmonize')

        for key, value in self.__dict__.items():
            logger.info('%s, %s', key, value)


HARMONIZE_CONFIG = HarmonizeConfig(CONFIG_FILE)
