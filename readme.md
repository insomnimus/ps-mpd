# PS-MPD
This Powershell module allows you to manage and control your [MPD](https://github.com/MusicPlayerDaemon/MPD) library and playback.

## Features
- Cross platform: runs on Linux, MacOSX and Windows.
- Full playback control.
- Structured access to your library: classes for tracks with their metadata, artists, albums and playlists.
- Cmdlets for managing MPD outputs.
- No external dependencies: the MPD protocol is implemented from scratch.
- Comes with great tab completions.
- As efficient as reasonable with a heavier emphasis on speed.
- New on version 2.0: ability to synchronize state in the background.

## Important
For many cmdlets from this module to work properly, you need to synchronize your session with the MPD server using the `Sync-MPD` cmdlet.

Also since you're likely going to be doing more things than control MPD on Powershell, this module does not keep a permanent connection to the MPD server; it connects when needed and then closes the connection.
This means that if you synchronized your session but modified MPD outside the module, you'll have to synchronize again, although no harm should be possible if you don't.

### Note on cmdlet naming
As you might know, Powershell wants everyone to name cmdlets (or functions) fitting the `Verb-Noun` format.
While every cmdlet in this module does that, Powershell also doesn't like if you use verbs unknown to it.
It will generate a warning upon loading modules with such cmdlet names:
> WARNING: The names of some imported commands from the module 'MPD' include unapproved verbs that might make them less discoverable. To find the commands with unapproved verbs, run the Import-Module command again with the Verbose parameter. For a list of approved verbs, type Get-Verb.                                                                            

While the list of verbs it knows is usually sufficient, it wasn't the case with this module. I had to choose between awkwardly named cmdlets and getting a warning (nobody would like `Start-Pause`).
Although the awkwardness could be avoided by changing the API (e.g. not `Pause-Playback` but `Set-Playback pause`), it isn't ergonomic for this module, especially if you do not wish to use a different MPD client like me.

The good news is there's a way to tell Powershell not to generate a warning. In your Powershell profile file, import the module like so:
```powershell
Import-Module MPD -DisableNameChecking
```

### Note on aliases
This module intentionally does not define any aliases. I expect people to have different opinions on naming so I'll leave you to define your own aliases in your Powershell profile.

## Tips
- You can have the module sync itself in the background when you start a new Powershell session without incurring the delay by adding to your profile:
	```powershell
	# The assignment to $null will prevent the output (which is an async Task) from being logged on your screen
	$null = Sync-MPD -Async
	```
- You might want to set some default parameter values for `Sync-MPD` as well:
	```powershell
	$PSDefaultParameterValues["Sync-MPD:PlaylistsDir" = "D:\Music\Playlists"]
	$PSDefaultParameterValues["Sync-MPD:MpdPassword"] = "1234"
	$PSDefaultParameterValues["Sync-MPD:MpdHost"] = "raspberrypi.local"
	# And so on
	```
- Many cmdlets in this module report back information messages using Powershell's information pipes. Since by default Powershell won't display these, I recommend setting these in your profile:
	```powershell
	$PSDefaultParameterValues["Play-Track:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Play-Artist:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Play-Album:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Play-Playlist:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Save-Playing:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Enable-MPDOutput:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Disable-MPDOutput:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Switch-MPDOutput:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Set-MPDOutputAttribute:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Set-MPDVolume:InformationAction"] = "Continue"
	$PSDefaultParameterValues["Seek-Queue:InformationAction"] = "Continue"
	```

	If you find that horrible to look at, you can simplify it by getting every command from this module and setting values:
	```powershell
	# Make sure `MPD` is loaded
	Get-Command -Module MPD | Foreach-Object {
		$PSDefaultParameterValues["$($_.Name):InformationAction"] = "Continue"
	}
	```
- If you use PSReadline (you likely do if you're on Powershell 7), you can assign key binds to virtually any action, including toggling playback:
	```powershell
		Set-PSReadLineKeyHandler -Chord Ctrl+Enter -Description "Toggle MPD playback" -ScriptBlock { Toggle-Playback }
	```

## Installation
### (Windows) With [Scoop](https://github.com/ScoopInstaller/Scoop)
1. Register my Scoop bucket:\
	`scoop bucket add insomnia https://github.com/insomnimus/scoop-bucket`
2. Install:\
	`scoop install ps-mpd`
3. The module will automatically load on new Powershell sessions but you can load it now with `Import-Module MPD -DisableNameChecking`.

Scoop will take care of updates of the module from then onwards.

### Manually download an archive
1. Go to the [releases page](https://github.com/insomnimus/ps-mpd) and download the most recent archive.
2. Go to a directory where Powershell looks for modules (check the `PSMODULEPATH` environment variable) and extract the archive into a directory called `MPD`. the directory should directly contain `MPD.psd1`.
3. The module will automatically load on new Powershell sessions but you can load it now with `Import-Module MPD -DisableNameChecking`.
4. You're responsible for updating it (remove old version, download and extract the archive, move to the same place).

### Build the module yourself
Prerequisites:
- The `dotnet` command line tool, version 8 or newer (might work on older versions but isn't tested)
- Powershell, to run the build script

1. Clone the repository:\
	`git clone https://github.com/insomnimus/ps-mpd`
2. Run the build script:
	```powershell
	cd ps-mpd
	./build.ps1
	```
3. If the build was successful, you'll have a directory containing the module written to `bin/MPD`.
4. Copy `bin/MPD` into a directory where Powershell looks for modules (check the `PSMODULEPATH` environment varaible).
5. The module will automatically load on new Powershell sessions but you can load it now with `Import-Module MPD -DisableNameChecking`.
6. You're responsible for updating it (remove old version, build, copy to the same place).

## Usage and documentation
The cmdlets come with built-in help which you can access by running `help <cmdlet-name>`.
Other than that, the cmdlet names are self explanatory.

For available parameters for a cmdlet, you should use the tab completion offered natively by Powershell.
