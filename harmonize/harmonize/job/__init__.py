import datetime
import asyncio
import threading
from typing import Any, Awaitable, Callable, Never, Tuple
import uuid
from harmonize.defs.job import Job, Status

Jobs: dict[uuid.UUID, Tuple[threading.Thread, Job]] = {}
async def send_file_to_laptop(file_name_with_dir):  
    pass

FuncType = Callable[[Any, Any], Awaitable[Any]]
async def start_job(job, input_args:Tuple[str]) -> Job:
    job_info: Job= {
        "id": uuid.uuid4(),
        "description": "idk",
        "started_on": datetime.datetime.now(),
        "status": Status.RUNNING
    }

    args = input_args + (job_info, )

    thread = threading.Thread(target=job, args=args)
    thread.start()

    Jobs[job_info["id"]] = (thread, job_info)

    return job_info       
