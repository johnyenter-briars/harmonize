import aiohttp


async def get(url: str, headers: dict[str, str], return_json: bool = False) -> tuple[int, str]:
    async with aiohttp.ClientSession() as session, session.get(url, headers=headers) as response:
        if return_json:
            return (response.status, await response.json())
        else:
            return (response.status, await response.text())
