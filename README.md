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
  - [X] MVP
  - [X] Initial caching
- [ ] Search media
- [ ] Stream any media
- [ ] Support fast forwarding
- [ ] Playlists
- [X] Search magnet links across different search platforms
- [X] Search youtube dl
  - [X] Simple keyword search
  - [ ] Simple file search and play
  - [ ] Save files in main DB
- [ ] search youtube dl in different regions
- [ ] Set / query long running jobs
  - [x] Basic read
  - [x] Basic cancel
  - [x] Job details / page
  - [X] Finish formatting edit page / list page
  - [ ] Jobs filter + order 
- [ ] option to integration with custom media system / fullsail
- [X] Option to download
- [ ] Implement https + private keys
- [ ] Remove all null ref warnings
- [ ] Fix all TODOs
- [ ] Log to file
- [X] Splash screen icon weirdly cut off

### Back End

- [x] Support fast forwarding
- [x] Test initial manual deployment on server
- [ ] Integrate with QBT
  - [X] MVP start / stop / pause 
  - [ ] Notice when file finished and save to server + metadata
- [ ] Integrate with youtube-dl
  - [x] MVP file search + download via API
  - [X] Save + query to DB
  - [ ] Metadata fixes
- [ ] Playlists
- [ ] Set / query long running jobs
  - [X] Update storing / saving jobs in database
- [ ] Integrate media art with youtube-dl
  - [X] MVP
  - [ ] Smart searching of artist metadata + cover art
- [X] Search magnet links across different search platforms
- [ ] Setup easy deployment / redeployment on server
- [ ] Test kill switch
- [ ] Implement https + private keys
- [ ] All 'async with ClientSession' use the 'get' helper in 'util'
- [ ] Fix all TODOs
- [ ] Run check for VPN when running QBT
