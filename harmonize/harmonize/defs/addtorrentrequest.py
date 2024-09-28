from pydantic import BaseModel


class AddTorrentRequest(BaseModel):
    magnet_links: list[str]
