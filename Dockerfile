FROM python:3.12

COPY ./harmonize /harmonize

ADD --chmod=755 https://astral.sh/uv/install.sh /install.sh

RUN /install.sh && rm /install.sh

COPY ./harmonize/requirements.txt requirements.txt

RUN /root/.cargo/bin/uv pip install --system --no-cache -r requirements.txt

WORKDIR /harmonize

CMD ["fastapi", "run", "main.py", "--port", "8000"]
