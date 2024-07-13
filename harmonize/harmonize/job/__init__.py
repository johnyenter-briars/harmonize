import datetime
import threading
import uuid
from collections.abc import Awaitable, Callable
from typing import Any

from harmonize.defs.job import Job, Status

Jobs: dict[uuid.UUID, tuple[threading.Thread, Job]] = {}


async def send_file_to_laptop(file_name_with_dir):  # noqa: ARG001
    pass


FuncType = Callable[[Any, Any], Awaitable[Any]]


async def start_job(job, input_args: tuple[str]) -> Job:
    job_info: Job = {
        'id': uuid.uuid4(),
        'description': 'idk',
        'started_on': datetime.datetime.now(datetime.UTC),
        'status': Status.RUNNING,
    }

    args = (*input_args, job_info)

    thread = threading.Thread(target=job, args=args)
    thread.start()

    Jobs[job_info['id']] = (thread, job_info)

    return job_info
