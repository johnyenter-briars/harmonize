from fastapi import FastAPI
from harmonize.router import download, metadata, search, stream, list, job

app = FastAPI()

app.include_router(download.router)
app.include_router(list.router)
app.include_router(metadata.router)
app.include_router(search.router)
app.include_router(stream.router)
app.include_router(job.router)


@app.get("/")
async def root():
    return {"message": "Hello from harmonize!"}