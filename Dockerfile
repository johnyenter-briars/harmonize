FROM python:3.12-slim

COPY --from=ghcr.io/astral-sh/uv:0.3.5 /uv /bin/uv

COPY ./harmonize /harmonize

ENV LD_LIBRARY_PATH=/usr/local/lib

WORKDIR /harmonize

RUN uv sync --frozen --no-cache

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ffmpeg \
    && apt-get clean && rm -rf /var/lib/apt/lists/*
