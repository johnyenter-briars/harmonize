import uuid

from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import Job, JobStatus
from harmonize.defs.response import BaseResponse
from harmonize.job.methods import cancel_job

router = APIRouter(prefix='/api')


@router.get('/job')
async def jobs(
    session: Session = Depends(get_session),
) -> BaseResponse[list[Job]]:
    statement = select(Job)
    jobs = list(session.exec(statement).all())

    return BaseResponse[list[Job]](message='Jobs found', status_code=200, value=jobs)


@router.get('/job/{job_id}')
async def get_job(
    job_id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    statement = select(Job).where(Job.id == uuid.UUID(job_id))
    job = session.exec(statement).first()

    if job is None:
        raise HTTPException(status_code=404, detail='Job not found in database')

    return BaseResponse[Job](message='Job found', status_code=200, value=job)


@router.post('/job/cancel/{job_id}')
async def cancel_job_req(
    job_id: str,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    statement = select(Job).where(Job.id == uuid.UUID(job_id))
    job = session.exec(statement).first()

    if job is None:
        raise HTTPException(status_code=404, detail='Job not found in database')

    await cancel_job(job_id)

    job.status = JobStatus.CANCELED

    session.add(job)
    session.commit()
    return BaseResponse[Job](message='Success', status_code=201, value=job)
