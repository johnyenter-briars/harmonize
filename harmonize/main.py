import asyncio
import logging
from contextlib import asynccontextmanager
from logging.config import dictConfig

from fastapi import Depends, FastAPI

import harmonize.config
import harmonize.config.harmonizeconfig
from harmonize.config.logconfig import LogConfig
from harmonize.db.database import get_session, seed
from harmonize.qbt.qbt import qbt_background_service
from harmonize.router import job, media, metadata, playlist, qbt, search, stream, youtube

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG

dictConfig(LogConfig().model_dump())

logger = logging.getLogger('harmonize')

config.log_config()


@asynccontextmanager
async def app_lifespan(app: FastAPI):
    background_services = [qbt_background_service]
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


app = FastAPI(lifespan=app_lifespan) if config.run_qbt else FastAPI()

app.include_router(youtube.router, dependencies=[Depends(get_session)])
app.include_router(media.router, dependencies=[Depends(get_session)])
app.include_router(metadata.router, dependencies=[Depends(get_session)])
app.include_router(search.router, dependencies=[Depends(get_session)])
app.include_router(stream.router, dependencies=[Depends(get_session)])
app.include_router(job.router, dependencies=[Depends(get_session)])
app.include_router(playlist.router, dependencies=[Depends(get_session)])

if config.run_qbt:
    app.include_router(qbt.router, dependencies=[Depends(get_session)])


seed()


@app.get('/')
async def root():
    return {'message': 'Hello from harmonize!'}
