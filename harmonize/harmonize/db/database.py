from collections.abc import Generator
from pathlib import Path
from typing import Any

from sqlmodel import Session, SQLModel, create_engine

import harmonize.config.harmonizeconfig

sqlite_file_name = 'database.db'
sqlite_url = f'sqlite:///{sqlite_file_name}'

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG

if config.reset_db_on_launch and Path.exists(Path(sqlite_file_name)):
    Path.unlink(Path(sqlite_file_name))

_engine = create_engine(sqlite_url)


def create_db_and_tables():
    SQLModel.metadata.create_all(_engine)


def get_session() -> Generator[Session, Any, None]:
    with Session(_engine) as session:
        yield session


def get_session_non_gen() -> Session:
    with Session(_engine) as session:
        return session


def create_db_tables():
    if Path.exists(Path(sqlite_file_name)):
        return

    SQLModel.metadata.drop_all(_engine)

    SQLModel.metadata.create_all(_engine)
