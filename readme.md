# PS-MPD
Powershell commands for the [Music Player Daemon (MPD)](https://github.com/MusicPlayerDaemon/MPD).

This module provides handy high-level commands to control MPD.

## Documentation
- [Brief overview](documentation.md)
- [List of commands](docs/)

## Features
- Tab complete titles, artists, albums and playlists.
- Load your library as Powershell objects.
- Manipulate your library and playlists.
- Play and queue tracks, artists, albums and playlists.

## Requirements
- The [mpc](https://github.com/MusicPlayerDaemon/mpc) tool needs to be installed and available in your `$PATH` as `mpc`.
- Powershell 5 or above. (Not tested on versions earlier than 7.2, however).

## Usage
1. Import the module either manually or in your Powershell profile: `Import-Module MPD`.
2. Load your music library: `Sync-MPD`.

Now you can use commands that work on your music library.

## Installation
### With Scoop (Recommended on Windows)
1. Add [my bucket](https://github.com/insomnimus/scoop-bucket) to scoop:
	`scoop bucket add insomnia https://github.com/insomnimus/scoop-bucket`
2. Update scoop:
	`scoop update`
3. Install the module:
	`scoop install ps-mpd`
4. (If you don't have mpd or mpc, you can get it from scoop as well):
	`scoop install mpd mpc`
5. Restart Powershell.

### Install Manually
```powershell
git clone https://github.com/insomnimus/ps-mpd
cd ps-mpd
# move `MPD` to somewhere where powershell automatically loads modules from
# e.g: `$env:PSModulePath.Split(";")[-1]`
```
