import datetime
import logging
import uuid
from collections.abc import Awaitable, Callable
from typing import Any

from fastapi import HTTPException
from sqlmodel import Session

from harmonize.db.models import Job, JobStatus
from harmonize.job.stoppablethread import StoppableThread

logger = logging.getLogger('harmonize')

Jobs: dict[uuid.UUID, tuple[StoppableThread, Job]] = {}

FuncType = Callable[[Any, Any, Any], Awaitable[Any]]


def _job_callback(job_function: Callable, args: Any, job: Job, session: Session):
    try:
        job_function(args, job, session)

        job.status = JobStatus.SUCCEEDED

    except Exception as e:
        job.status = JobStatus.FAILED
        job.error_message = str(e)
        logger.exception(e)
    finally:
        session.add(job)
        session.commit()


async def start_job(
    job_key: str, description: str, job_function: Callable, session: Session, input_args: tuple[Any]
) -> Job:
    # TODO: should this create a new process scoped session?
    job = Job(
        key=job_key,
        description=description,
        status=JobStatus.RUNNING,
        started_on=datetime.datetime.now(datetime.UTC),
        error_message=None,
    )

    session.add(job)
    session.commit()

    callback_args = (job_function, *input_args, job, session)

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
