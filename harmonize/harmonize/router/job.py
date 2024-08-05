from fastapi import APIRouter

from harmonize.defs.job import Job
from harmonize.defs.response import BaseResponse
from harmonize.job.methods import Jobs, cancel_job

router = APIRouter(prefix='/api')


@router.get('/job')
async def jobs() -> list[Job]:
    return [job_value[1] for job_value in Jobs.values()]


@router.post('/job/cancel/{job_id}')
async def cancel_job_req(job_id) -> BaseResponse[Job]:
    updated_job = await cancel_job(job_id)
    return {'message': 'Job canceled', 'status_code': 201, 'value': updated_job}
