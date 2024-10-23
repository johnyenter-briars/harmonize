from pathlib import Path

from sqlmodel import Session, SQLModel, create_engine

sqlite_file_name = 'database.db'
sqlite_url = f'sqlite:///{sqlite_file_name}'

engine = create_engine(sqlite_url)


def create_db_and_tables():
    SQLModel.metadata.create_all(engine)


def get_session():
    with Session(engine) as session:
        yield session


def seed():
    if Path.exists(Path(sqlite_file_name)):
        return

    SQLModel.metadata.drop_all(engine)

    SQLModel.metadata.create_all(engine)
