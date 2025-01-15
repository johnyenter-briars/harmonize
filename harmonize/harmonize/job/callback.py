import datetime
import uuid
from collections.abc import Awaitable, Callable
from typing import Any

from fastapi import HTTPException
from sqlmodel import Session

from harmonize.db.models import Job, JobStatus
from harmonize.job.stoppablethread import StoppableThread

Jobs: dict[uuid.UUID, tuple[StoppableThread, Job]] = {}


FuncType = Callable[[Any, Any, Any], Awaitable[Any]]


def _job_callback(job_function: Callable, args: Any, job: Job, session: Session):
    try:
        job_function(args, job, session)

        job.status = JobStatus.SUCCEEDED

    except Exception as e:
        job.status = JobStatus.FAILED
        job.error_message = str(e)
    finally:
        session.add(job)
        session.commit()


async def start_job(
    description: str, job_function: Callable, session: Session, input_args: tuple[Any]
) -> Job:
    job = Job(
        description=description,
        status=JobStatus.RUNNING,
        started_on=datetime.datetime.now(datetime.UTC),
        error_message=None,
    )

    session.add(job)
    session.commit()

    args = (*input_args, job, session)
    callback_args = (job_function, *input_args, job, session)

    # thread = StoppableThread(target=job_function, args=args)
    thread = StoppableThread(target=_job_callback, args=callback_args)
    thread.start()

    Jobs[job.id] = (thread, job)

    return job


async def cancel_job(job_id: str) -> None:
    parsed_id = uuid.UUID(job_id)

    job = Jobs.get(parsed_id)
    if job is None:
        raise HTTPException(status_code=404, detail='Job not available to cancel')
    (thread, job) = job

    thread.stop()
    thread.join()
