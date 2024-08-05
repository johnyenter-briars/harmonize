import datetime
import uuid
from collections.abc import Awaitable, Callable
from typing import Any

from harmonize.defs.job import Job, Status
from harmonize.job.stoppablethread import StoppableThread

Jobs: dict[uuid.UUID, tuple[StoppableThread, Job]] = {}


FuncType = Callable[[Any, Any], Awaitable[Any]]


async def start_job(job_function, input_args: tuple[str]) -> Job:
    job_info: Job = {
        'id': uuid.uuid4(),
        'description': 'idk',
        'started_on': datetime.datetime.now(datetime.UTC),
        'status': Status.RUNNING,
    }

    args = (*input_args, job_info)

    thread = StoppableThread(target=job_function, args=args)
    thread.start()

    Jobs[job_info['id']] = (thread, job_info)

    return job_info


async def cancel_job(job_id: str) -> Job:
    parsed_id = uuid.UUID(job_id)
    (thread, job) = Jobs[parsed_id]

    thread.stop()
    thread.join()

    job['status'] = Status.Canceled

    return job
