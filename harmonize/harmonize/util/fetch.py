import aiohttp


async def get(url: str, headers: dict[str, str]) -> tuple[int, str]:
    async with aiohttp.ClientSession() as session, session.get(url, headers=headers) as response:
        return (response.status, await response.text())
