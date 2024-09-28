from typing import TypedDict


class BaseResponse[T](TypedDict):
    message: str
    status_code: int
    value: T | None
