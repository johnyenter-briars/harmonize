from sqlmodel import SQLModel

from harmonize.db.database import engine


def seed():
    SQLModel.metadata.drop_all(engine)

    SQLModel.metadata.create_all(engine)
