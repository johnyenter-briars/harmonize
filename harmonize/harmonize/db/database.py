from collections.abc import Generator
from pathlib import Path
from typing import Any

from sqlalchemy.orm import sessionmaker
from sqlmodel import Session, SQLModel, create_engine

import harmonize.config.harmonizeconfig

sqlite_file_name = 'database.db'
sqlite_url = f'sqlite:///{sqlite_file_name}'

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG

if config.reset_db_on_launch and Path.exists(Path(sqlite_file_name)):
    Path.unlink(Path(sqlite_file_name))

_engine = create_engine(sqlite_url)

SessionLocal = sessionmaker(bind=_engine, class_=Session, autoflush=False, autocommit=False)


def get_new_session() -> Session:
    """
    Returns a new session instance, ensuring each process gets its own.
    """
    return SessionLocal()


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
