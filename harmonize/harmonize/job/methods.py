import uuid
from collections.abc import Awaitable, Callable
from typing import Any

from fastapi import HTTPException
from sqlmodel import Session

from harmonize.db.models import Job
from harmonize.job.stoppablethread import StoppableThread

Jobs: dict[uuid.UUID, tuple[StoppableThread, Job]] = {}


FuncType = Callable[[Any, Any], Awaitable[Any]]


async def start_job(
    job_function: Callable, job: Job, session: Session, input_args: tuple[str]
) -> Job:
    args = (*input_args, job, session)

    thread = StoppableThread(target=job_function, args=args)
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
