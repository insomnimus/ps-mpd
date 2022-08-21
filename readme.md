# PS-MPD
Powershell commands for the [Music Player Daemon (MPD)](https://github.com/MusicPlayerDaemon/MPD).

This module provides handy high-level commands to control MPD.

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
2. Load your music library: `Sync-Mpd`.

Now you can use commands that work on your music collection. Documentation coming soon.

## Installation
```powershell
git clone https://github.com/insomnimus/ps-mpd mpd
# move `mpd` to somewhere where powershell automatically loads modules from
# e.g: `$env:PSModulePath.Split(";")[-1]`
```
