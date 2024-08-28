import datetime

from sqlmodel import Session, SQLModel, select

from harmonize.db.database import engine
from harmonize.db.models import Job, Status


def main():
    SQLModel.metadata.drop_all(engine)
    job_1 = Job(
        description='A Failed Job',
        status=Status.FAILED,
        started_on=datetime.datetime.now(datetime.UTC),
    )
    job_2 = Job(
        description='A Running Job',
        status=Status.RUNNING,
        started_on=datetime.datetime.now(datetime.UTC),
    )
    job_3 = Job(
        description='A Succeeded Job',
        status=Status.SUCCEEDED,
        started_on=datetime.datetime.now(datetime.UTC),
    )

    SQLModel.metadata.create_all(engine)

    with Session(engine) as session:
        session.add(job_1)
        session.add(job_2)
        session.add(job_3)
        session.commit()
        statement = select(Job).where()
        jobs = session.exec(statement).all()
        print(jobs)


main()
