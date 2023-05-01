# PS-MPD Documentation
This module exposes convenient commands and tab completions for the Music Player Daemon (MPD).

This page documents available commands briefly. All the commands and their usage is self-explanatory and for the parameters you can call `Get-Help`.

One thing to note is this module works in-memory, that is, the music library is loaded into memory (only the metadata) and serialized into pwoershell objects.

The reasoning behind it is with this approach tab completions work fast even on huge libraries.

However this means that the state of MPD and the module can get out of sync.
So, everytime you open a new Powershell session or modify MPD, you need to call `Sync-MPD`.

This module does not attempt to remedy badly tagged music. This means it does not look at the directory structure of your music folder.

The metadata this module keeps is as follows:
- Title
- Album
- Artist
- Duration
- Track No (if available, else ignored)
- File (mpc needs the file path as MPD knows - to play songs)

However, even tracks without any tags will be loaded and available.

## Playlists
PS-MPD does not enforce any playlist file format but defers to MPD's default: m3u.
The format recognized is the same as MPD.

However, you need to specify a playlists directory to `Sync-MPD` with the `-PlaylistsDir` parameter if you want to work with your playlists.

For convenience I recommend you set this in `$PSDefaultParameterValues`, an automatic variable built-into Powershell.
For example, somewhere in your Powershell profile:
```powershell
$PSDefaultParameterValues["Sync-MPD:PlaylistsDir"] = "D:\music\playlists"
```

The m3u files under the directory you specified will be recognized as playlists.

## Enabling Nicer Information Messages
Since Powershell by default does not show messages written with `Write-Information`, I suggest you enable them at for this modules commands, again with `$PSDefaultParameterValues`.

For example:

```powershell
$PSDefaultParameterValues["*-Playlist":InformationAction] =
	$PSDefaultParameterValues["*-Track":InformationAction] =
	$PSDefaultParameterValues["Play-Album:InformationAction"] =
	$PSDefaultParameterValues["Play-Artist:InformationAction"] = "Continue"
```

(Yes, that syntax is valid.)

## Commands Overview
### [Get-Album](docs/Get-Album.md)
Aliases:
- `salb`
Gets an album.

### [Play-Album](docs/Play-Album.md)
Aliases:
- `palb`
Plays an album.
 You can pipe Album objects into this command.

### [Get-Artist](docs/Get-Artist.md)
Aliases:
- `sart`
Gets an artist.

### [Play-Artist](docs/Play-Artist.md)
Aliases:
- `part`
Plays all tracks by an artist.

### [Sync-Mpd](docs/Sync-Mpd.md)

Syncs the MPD database with the current Powershell session.
 The MPD music library is kept in-memory (just the metadata) for fast tab completions and operations. Use this command to synchronize the current state of MPD with your Powershell session.
 This is not permanent; you need to call this command each time you open up a new session. You also need to call this command if the MPD database changes.
 For playlists to be recognized, you need to specify the `-PlaylistsDir` parameter. Files with the `m3u` extension under this directory will be loaded as playlists.

### [Get-MPDStatus](docs/Get-MPDStatus.md)
Aliases:
- `cur`
- `ü`
Gets the MPD playback status.

### [Play-Next](docs/Play-Next.md)
Aliases:
- `>`
Plays the next track in the queue.

### [Save-Playing](docs/Save-Playing.md)
Aliases:
- `save`
- `sp`
Saves the currently playing track to a playlist.

### [Get-Playlist](docs/Get-Playlist.md)
Aliases:
- `spla`
Gets a playlist.
 For this command to work, the last call to `Sync-MPD` must have set the `-PlaylistsDir` parameter to a directory containing `.m3u` files.

### [Save-Playlist](docs/Save-Playlist.md)

Saves a playlist to disk.
 By default commands that modify playlists will save the changes back to the disk. This command is included for completeness.

### [Play-Playlist](docs/Play-Playlist.md)
Aliases:
- `pl`
Plays a playlist.

### [Play-Previous](docs/Play-Previous.md)
Aliases:
- `<`
Plays the previous track in the queue.

### [Seek-Queue](docs/Seek-Queue.md)
Aliases:
- `seek`
Seeks to a track in the current queue.

### [Select-Track](docs/Select-Track.md)
Aliases:
- `slt`
Filters tracks by title, artist and album.
 Every parameter specified will narrow down the filter. Specifying multiple values per-parameter will make the filter more general.

### [Play-Track](docs/Play-Track.md)
Aliases:
- `ptra`
Plays a track.

### [Remove-Track](docs/Remove-Track.md)
Aliases:
- `rt`
Removes tracks from a playlist.

### [Save-Track](docs/Save-Track.md)

Saves one or more tracks to a playlist.

### [Get-Track](docs/Get-Track.md)
Aliases:
- `stra`
Gets a track.
