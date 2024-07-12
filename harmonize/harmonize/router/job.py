from fastapi import APIRouter
from harmonize.defs.job import Job
from harmonize.job import Jobs

router = APIRouter()


@router.get("/job")
async def jobs() -> list[Job]:
    return [job_value[1] for job_value in Jobs.values()]