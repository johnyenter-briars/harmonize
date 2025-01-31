# harmonize

### To Run Locally:

Good luck.

## Todos For Audio Support Only

- [X] Delete video method
- [X] Edit element page
- [X] Job List page
  - [X] Item tapped color
  - [X] Bug where two identical elements show up
  - [X] Sort elements by started time
  - [X] If transfer, status of transfer
- [ ] Media List
  - [ ] Nice search
  - [ ] Season search
  - [ ] Filter + query popup
- [ ] Health page
  - [X] Add page
  - [X] VPN check
  - [ ] Network check
- [ ] Test + confirm QBT
- [X] Seasons backend
- [ ] Seasons frontend
  - [ ] Edit Season Name
  - [ ] Display entries in season
- [ ] Update colors
- [X] Hook up send to media server
- [ ] Backfill method
- [ ] Update server
- [ ] Setup daemon
- [ ] Setup SSL Cert + Https

## Todos For Full Functionality

### Front End

- [ ] List media
  - [X] MVP
  - [X] Initial caching
  - [ ] Smarter caching
  - [ ] Show progress bar when cache is syncing
  - [ ] Group by album
- [ ] Search media
  - [ ] Search by name
  - [ ] Search album by name
  - [ ] Search playlist by name
- [ ] Stream any media
  - [X] Audio stream
  - [ ] Video stream
- [ ] Support fast forwarding
  - [X] Fast forwarding media element
  - [ ] Fast forwarding via dropdown
- [X] Search magnet links across different search platforms
- [X] Search youtube dl
  - [X] Simple keyword search
  - [X] Simple file search and play
  - [X] Save files in main DB
- [ ] search youtube dl in different regions
- [X] Set / query long running jobs
  - [x] Basic read
  - [x] Basic cancel
  - [x] Job details / page
  - [X] Finish formatting edit page / list page
  - [X] Jobs filter + order 
- [ ] option to integration with custom media system / fullsail
  - [X] Optional media remote 
  - [ ] Optional send to media system 
- [X] Option to download
- [ ] Implement https + private keys
- [ ] Remove all null ref warnings
- [ ] Fix all TODOs
- [X] Log to file
- [X] Splash screen icon weirdly cut off
- [ ] Display storage space
  - [ ] Local storage 
  - [ ] Remote storage 
- [ ] Update layout (copy spotify)
  - [ ] App shell uses icon bar at bottom - remove title at top
  - [ ] Nicer pages / navigation to get to system pages
  - [ ] Media element can collapse to rectangle at bottom
  - [ ] Home page shows quick links as tiles


### Back End

- [x] Support fast forwarding
- [x] Test initial manual deployment on server
- [ ] Integrate with QBT
  - [X] MVP start / stop / pause 
  - [X] Notice when file finished and save to server + metadata
- [ ] Integrate with youtube-dl
  - [x] MVP file search + download via API
  - [X] Save + query to DB
  - [ ] Metadata fixes
- [ ] Playlists
- [ ] Set / query long running jobs
  - [X] Update storing / saving jobs in database
  - [ ] Refactor to use fast API background tasks 
- [ ] Integrate media art with youtube-dl
  - [X] MVP
  - [ ] Smart searching of artist metadata + cover art
  - [ ] Crop images instead of squashing
- [X] Search magnet links across different search platforms
- [ ] Setup easy deployment / redeployment on server
  - [X] Initial zip test
  - [ ] Script zip steps a bask / ps script
- [ ] Test kill switch
- [ ] Implement https + private keys
- [ ] All 'async with ClientSession' use the 'get' helper in 'util'
- [ ] Display storage space
  - [ ] Local storage 
  - [ ] Remote storage 
- [ ] Fix all TODOs
- [ ] Run check for VPN when running QBT
