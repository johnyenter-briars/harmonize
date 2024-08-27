import logging
from logging.config import dictConfig

from fastapi import FastAPI

import harmonize.config
import harmonize.config.harmonizeconfig
from harmonize.config.logconfig import LogConfig
from harmonize.router import download, job, list, metadata, playlist, search, stream  # type: ignore

config = harmonize.config.harmonizeconfig.HARMONIZE_CONFIG

dictConfig(LogConfig().model_dump())

logger = logging.getLogger('harmonize')

config.log_config()

app = FastAPI(debug=True)

app.include_router(download.router)
app.include_router(list.router)
app.include_router(metadata.router)
app.include_router(search.router)
app.include_router(stream.router)
app.include_router(job.router)
app.include_router(playlist.router)


@app.get('/')
async def root():
    return {'message': 'Hello from harmonize!'}
