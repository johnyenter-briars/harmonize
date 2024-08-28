import logging
from logging.config import dictConfig

from fastapi import Depends, FastAPI

from harmonize.config import LogConfig
from harmonize.db.database import get_session
from harmonize.router import download, job, list, metadata, playlist, search, stream

dictConfig(LogConfig().model_dump())
logger = logging.getLogger('harmonize')


app = FastAPI()
app.include_router(download.router, dependencies=[Depends(get_session)])
app.include_router(list.router, dependencies=[Depends(get_session)])
app.include_router(metadata.router, dependencies=[Depends(get_session)])
app.include_router(search.router, dependencies=[Depends(get_session)])
app.include_router(stream.router, dependencies=[Depends(get_session)])
app.include_router(job.router, dependencies=[Depends(get_session)])
app.include_router(playlist.router, dependencies=[Depends(get_session)])


@app.get('/')
async def root():
    return {'message': 'Hello from harmonize!'}
