from pydantic import BaseModel, ConfigDict
from pydantic.alias_generators import to_camel
from sqlmodel import SQLModel


class BaseSchema(BaseModel):
    model_config = ConfigDict(
        alias_generator=to_camel,
        populate_by_name=True,
        from_attributes=True,
    )

    def dict(self, **kwargs):
        """Custom dict method to include relationships."""
        exclude = kwargs.pop('exclude', set())
        obj_dict = super().dict(**kwargs)

        # Handle relationships explicitly
        for key, value in self.__dict__.items():
            if key not in exclude:
                if isinstance(value, list):
                    # Serialize lists of related objects
                    obj_dict[key] = [item.dict() for item in value if isinstance(item, SQLModel)]
                elif isinstance(value, SQLModel):
                    # Serialize single related objects
                    obj_dict[key] = value.dict()
        return obj_dict


class BaseResponse[T](BaseSchema):
    message: str
    status_code: int
    value: T | None
