import asyncio
import base64
import logging
from contextlib import asynccontextmanager
from datetime import datetime
from logging.config import dictConfig

from fastapi import Depends, FastAPI
from fastapi.responses import JSONResponse
from starlette.requests import Request

import harmonize.config
import harmonize.config.harmonizeconfig
import harmonize.config.harmonizesecrets
from harmonize.config.logconfig import LogConfig
from harmonize.db.database import create_db_tables, get_session
from harmonize.qbt.qbt import qbt_background_service
from harmonize.router import (
    health,
    job,
    media,
    metadata,
    playlist,
    qbt,
    search,
    season,
    stream,
    transfer,
)

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG
secrets = harmonize.config.harmonizesecrets.HARMONIZE_SECRETS

dictConfig(LogConfig().model_dump())

logger = logging.getLogger('harmonize')

config.log_config()


@asynccontextmanager
async def app_lifespan(_: FastAPI):
    background_services = [qbt_background_service] if config.run_qbt else []

    tasks = []

    for background_service in background_services:
        logger.info('Starting background service %s...', background_service.__name__)
        tasks.append(asyncio.create_task(background_service()))

    yield

    logger.info('Shutting down background service...')

    for task in tasks:
        task.cancel()
        try:
            await task
        except asyncio.CancelledError:
            logger.info('Background service cancelled.')


app = FastAPI(lifespan=app_lifespan, dependencies=[Depends(get_session)])

app.state.start_time = datetime.now()

app.include_router(media.router, dependencies=[Depends(get_session)])
app.include_router(metadata.router, dependencies=[Depends(get_session)])
app.include_router(search.router, dependencies=[Depends(get_session)])
app.include_router(stream.router, dependencies=[Depends(get_session)])
app.include_router(job.router, dependencies=[Depends(get_session)])
app.include_router(playlist.router, dependencies=[Depends(get_session)])
app.include_router(season.router, dependencies=[Depends(get_session)])
app.include_router(transfer.router, dependencies=[Depends(get_session)])
app.include_router(health.router, dependencies=[Depends(get_session)])

if config.run_qbt:
    app.include_router(qbt.router, dependencies=[Depends(get_session)])


def check_permission(method, api, auth):
    scheme, data = (auth or ' ').split(' ', 1)
    if scheme != 'Basic':
        return False

    username, password = base64.b64decode(data).decode().split(':', 1)

    return username == secrets.harmonize_username and password == secrets.harmonize_password


if config.enable_auth:

    @app.middleware('http')
    async def check_authentication(request: Request, call_next):
        auth = request.headers.get('Authorization')
        if not check_permission(request.method, request.url.path, auth):
            return JSONResponse(None, 401, {'WWW-Authenticate': 'Basic'})
        return await call_next(request)


create_db_tables()
