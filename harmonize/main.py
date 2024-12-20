import logging
from logging.config import dictConfig

from fastapi import Depends, FastAPI

import harmonize.config
import harmonize.config.harmonizeconfig
from harmonize.config.logconfig import LogConfig
from harmonize.db.database import get_session, seed
from harmonize.router import job, media, metadata, playlist, qbt, search, stream, youtube

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG

dictConfig(LogConfig().model_dump())

logger = logging.getLogger('harmonize')

config.log_config()

app = FastAPI()
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
