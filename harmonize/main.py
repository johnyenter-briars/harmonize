import logging
from logging.config import dictConfig

from fastapi import FastAPI

from harmonize.config import LogConfig
from harmonize.router import download, job, list, metadata, search, stream

dictConfig(LogConfig().model_dump())
logger = logging.getLogger('harmonize')

app = FastAPI(debug=True)

app.include_router(download.router)
app.include_router(list.router)
app.include_router(metadata.router)
app.include_router(search.router)
app.include_router(stream.router)
app.include_router(job.router)


@app.get('/')
async def root():
    return {'message': 'Hello from harmonize!'}


# LOGGING_CONFIG = {
#     'version': 1,
#     'disable_existing_loggers': False,
#     'formatters': {
#         'default': {
#             '()': 'logging.Formatter',
#             'fmt': '%(levelname)s %(name)s@%(lineno)d %(message)s',
#         },
#     },
#     'handlers': {
#         'default': {
#             'formatter': 'default',
#             'class': 'my_project.ColorStreamHandler',
#             'stream': 'ext://sys.stderr',
#         },
#     },
#     'loggers': {
#         '': {'handlers': ['default'], 'level': 'TRACE'},
#     },
# }

# if __name__ == '__main__':
#     import uvicorn

#     uvicorn.run(app, log_config=LOGGING_CONFIG)
