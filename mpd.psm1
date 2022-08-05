using namespace System.Collections.Generic

$script:MPD = [Mpd]::new()

class Track {
	[string] $title
	[string] $artist
	[string] $album
	[string] $file
}

class Mpd {
	[List[Track]]
	$tracks = [List[Track]]::new(1024)
	[SortedDictionary[string, [List[Track]]]]
	$artists = [SortedDictionary[string, [List[Track]]]]::new([StringComparer]::InvariantCultureIgnoreCase)

	[void] reload() {
		$fmt = "%artist%::%title%::%album%::%file%"
		$this.artists.clear()
		$this.tracks.clear()

		foreach($val in mpc.exe -q -f $fmt listall) {
			$val = $val.trim()
			$artist, $title, $album, $file = $val.split("::") | foreach-object { $_.trim() }
			if($null -eq $file) {
				continue
			}
			$song = [Track] @{ title = $title; artist = $artist; file = $file; album = $album }
			$artistTracks = [List[Track]]::new()
			if($this.artists.TryGetValue($artist, [ref] $artistTracks)) {
				[void] $artistTracks.Add($song)
			} else {
				$artistTracks = $artistTracks = [List[Track]]::new(32)
				[void] $artistTracks.Add($song)
				[void] $this.artists.Add($artist, $artistTracks)
			}
			[void] $this.tracks.Add($song)
		}
	}
}

function :mpc {
	[cmdletBinding()]
	[outputType([string[]])]
	param (
		[parameter(position = 0, valueFromRemainingArguments)]
		[object[]] $Command
	)
	mpc.exe -q $args
	if($LastExitCode -ne 0) {
		write-error "mpc process exited with $lastExitCode"
	}
}

function Get-Track {
	[CmdletBinding()]
	[OutputType([Track])]
	param (
		[parameter(position = 0)]
		[string] $Title,
		[parameter()]
		[string] $Artist,
		[parameter()]
		$Album
	)

	foreach($x in $script:MPD.tracks) {
		if(
			(!$artist -or $x.artist -like $artist) -and
			(!$album -or $x.album -like $album) -and
			(!$title -or $x.title -like $title)
		) {
			$x
		}
	}
}

function get-mpd { return $script:mpd }
function reload-mpd { $script:MPD.reload() }
