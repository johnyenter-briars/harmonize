import uuid

from pydantic import BaseModel, Field


class UpsertSeasonRequest(BaseModel):
    name: str


class AssociateToSeasonRequest(BaseModel):
    season_id: uuid.UUID = Field(alias='seasonId')
    media_entry_ids: list[uuid.UUID] = Field(alias='mediaEntryIds')


class DisassociateToSeasonRequest(BaseModel):
    season_id: uuid.UUID = Field(alias='seasonId')
    media_entry_ids: list[uuid.UUID] = Field(alias='mediaEntryIds')
