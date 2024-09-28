# harmonize

### To Run Locally:

Install uv (https://docs.astral.sh/uv/getting-started/installation/)

`cd` into the `harmonize` directory

Run `uv run fastapi dev .\main.py`

### To run with docker:

NOTE: This is for running with docker for devleopment.

`docker-compose --profile dev up`

## todos

### Front End

- [ ] List media
- [ ] Search media
- [ ] Stream any media
- [ ] Support fast forwarding
- [ ] Playlists
- [ ] Search magnet links across different search platforms
- [ ] Search youtube dl
  - [ ] Simple keyword search
  - [ ] Simple file search and play
  - [ ] Save files in main DB
- [ ] search youtube dl in different regions
- [ ] Set / query long running jobs
  - [x] Basic read
  - [x] Basic cancel
  - [x] Job details / page
  - [ ] Job details update
  - [ ] Finish formatting edit page / list page
- [ ] option to integration with custom media system / fullsail
- [ ] Option to download
- [ ] Implement https + private keys
- [ ] Splash screen icon weirdly cut off

### Back End

- [x] Support fast forwarding
- [x] Test initial manual deployment on server
- [ ] Integrate with QBT
- [ ] Integrate with youtube-dl
  - [x] MVP file search + download via API
  - [ ] Save + query to DB
- [ ] Playlists
- [x] Set / query long running jobs
- [ ] Update storing / saving jobs in database
- [ ] Integrate media art with youtube-dl
- [ ] Search magnet links across different search platforms
- [ ] Setup easy deployment / redeployment on server
- [ ] Automatic media art when not found?
- [ ] Test kill switch
- [ ] Implement https + private keys
