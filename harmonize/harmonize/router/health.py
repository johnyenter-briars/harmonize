from datetime import datetime

from fastapi import APIRouter, Depends, Request
from sqlmodel import Session, select

from harmonize.db.database import get_session
from harmonize.db.models import Job
from harmonize.defs.health import HealthStatus, Uptime
from harmonize.defs.response import BaseResponse

router = APIRouter(prefix='/api/health')


@router.get('/')
async def jobs(
    session: Session = Depends(get_session),
) -> BaseResponse[list[Job]]:
    statement = select(Job)
    jobs = list(session.exec(statement).all())

    return BaseResponse[list[Job]](message='Jobs found', status_code=200, value=jobs)


@router.get('/status')
async def status(request: Request) -> BaseResponse[HealthStatus]:
    start_time = request.app.state.start_time
    current_time = datetime.now()
    uptime = current_time - start_time
    uptime_seconds = int(uptime.total_seconds())
    hours, remainder = divmod(uptime_seconds, 3600)
    minutes, seconds = divmod(remainder, 60)

    uptime = Uptime(seconds=uptime_seconds, hours=hours, minutes=minutes)

    status = HealthStatus(uptime=uptime, drives=[])

    return BaseResponse[HealthStatus](message='Status', status_code=200, value=status)
