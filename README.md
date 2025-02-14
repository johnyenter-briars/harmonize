# harmonize

### To Run Locally:

Good luck.

## Todos For Video Support Only

- [X] Delete video method
- [X] Edit element page
- [X] Job List page
  - [X] Item tapped color
  - [X] Bug where two identical elements show up
  - [X] Sort elements by started time
  - [X] If transfer, status of transfer
- [X] Media List
  - [X] Nice search
  - [X] Filter + query popup
  - [X] Filter by type
  - [X] Filter by transferred
  - [X] Order by added on
  - [X] Delete entry (delete file on disk)
  - [X] Deletes children
  - [X] Paging
  - [X] Display SRT files
  - [X] subtitle rename....?
- [X] Health page
  - [X] Add page
  - [X] VPN check
  - [X] Network check
- [X] Test + confirm QBT
  - [X] Individual eps in a folder considered a season
  - [X] Add ability to tag
- [X] Seasons backend
- [X] Seasons frontend
  - [X] Edit Season Name
  - [X] Add season
  - [X] Display entries in season
  - [X] Delete season
  - [X] Paging
  - [X] Edit entry from within season
  - [X] Order by added on
  - [X] Associate to season
  - [X] Disssociate to season
- [X] Control
  - [X] Button to open up videos
  - [X] Button to open up Youtube
- [X] Update colors globally
  - [X] Media control colors
- [X] Alerts should be snackbar
- [X] Style snackbar
- [X] Fix paging on seasons
- [X] Fix paging on media entry
- [X] Add new properties to edit media entry
- [X] Hook up send to media server
- [X] Gb rounding in QBT
- [X] Remove files from media system
- [X] Qbt job doesn't lock up requests
- [X] Authentication
- [X] Only most recent 50 jobs
- [X] Only most recent 50 transferrs
- [X] Opening search bar focuses bar - season + video
- [X] Clicking on transfer element brings you to edit page
- [X] Delete of entry remotely also deletes its subtitle file(s) (if present)
- [ ] Setup SSL Cert + Https
- [ ] Backfill method
- [ ] Update server
- [ ] Setup daemon
- [ ] Fix YT
- [ ] IOS

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
