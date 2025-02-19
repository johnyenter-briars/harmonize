import uuid

from fastapi import APIRouter, Depends, HTTPException, Query
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import Job, JobStatus
from harmonize.defs.response import BaseResponse
from harmonize.job.callback import cancel_job

router = APIRouter(prefix='/api')


@router.get('/job')
async def jobs(
    limit: int = Query(50, ge=1),
    skip: int = Query(0, ge=0),
    session: Session = Depends(get_session),
) -> BaseResponse[list[Job]]:
    statement = select(Job)

    statement = statement.offset(skip).limit(limit)

    statement = statement.order_by(Job.started_on.desc())  # type: ignore

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


@router.post('/job/cancel/{job_id}', status_code=201)
async def cancel_job_req(
    job_id: uuid.UUID,
    session: Session = Depends(get_session),
) -> BaseResponse[Job]:
    job = session.get(Job, job_id)

    if job is None:
        raise HTTPException(status_code=404, detail='Job not found in database')

    await cancel_job(job_id)

    job.status = JobStatus.CANCELED

    session.add(job)
    session.commit()
    return BaseResponse[Job](message='Success', status_code=201, value=job)
