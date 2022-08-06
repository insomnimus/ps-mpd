using namespace System.Collections.Generic

$script:MPD = [Mpd]::new()
$script:fmt = "%artist%`u{1}%title%`u{1}%album%`u{1}%file%"

$ExecutionContext.SessionState.Module.OnRemove += {
	$script:MPD.arttists.clear()
	$script:MPD = $null
}

class Track {
	[string] $Title
	[string] $Artist
	[string] $Album
	[string] $File

	static [Track] Parse([string]$s) {
		$c = [char]1
		$_artist, $_title, $_album, $_file = $s.split($c) | foreach-object { $_.trim() }
		if($null -eq $_file) {
			return $null
		}
		$track = [Track] @{
			Title = $_title
			Artist = $_artist
			Album = $_Album
			File = $_file
		}
		return $track
	}

	[string] ToString() {
		$art = $this.artist ?? "?"
		$alb = $this.album ?? "?"
		$tit = $this.title ?? $this.file
		return "$tit [$alb] by $art"
	}

	[void] Play() {
		script:Play-Track $this
	}

	[void] Queue() {
		script:Play-Track -queue $this
	}
}

class Mpd {
	# [List[Track]] $tracks = [List[Track]]::new(1024)
	[SortedDictionary[string, [List[Track]]]]
	$artists = [SortedDictionary[string, [List[Track]]]]::new([StringComparer]::InvariantCultureIgnoreCase)

	[void] reload() {
		$this.artists.clear()

		foreach($val in script::mpc -f $script:fmt listall) {
			$song = [Track]::parse($val)
			if(!$song) {
				continue
			}

			$artistTracks = [List[Track]]::new()
			if($this.artists.TryGetValue($song.artist, [ref] $artistTracks)) {
				[void] $artistTracks.Add($song)
			} else {
				$artistTracks = [List[Track]]::new(32)
				[void] $artistTracks.Add($song)
				[void] $this.artists.Add($song.artist, $artistTracks)
			}
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
	mpc.exe -q $command
	if($LastExitCode -ne 0) {
		write-error "mpc process exited with $lastExitCode"
	}
}

function Get-Track {
	[CmdletBinding(DefaultParameterSetName = "query")]
	[OutputType([Track])]
	param (
		[parameter(
			ParameterSetName = "query",
			position = 0,
			HelpMessage = "Title of the track"
		)]
		[string] $Title,
		[parameter(
			ParameterSetName = "query",
			HelpMessage = "Name of the artist"
		)]
		[string] $Artist,
		[parameter(
			ParameterSetName = "query",
			HelpMessage = "Name of the album"
		)]
		$Album,

		[Parameter(Mandatory, ParameterSetName = "current", HelpMessage = "Get the currently playing song or playlist")]
		[ValidateSet("Track", "Playlist")]
		[string]$Current
	)

	if($current -eq "Track") {
		script::mpc -f $script:fmt current | foreach-object { [Track]::parse($_) }
		return
	} elseif($current -eq "Playlist") {
		script::mpc -f $fmt playlist | foreach-object { [Track]::Parse($_) }
		return
	}

	foreach($x in $script:MPD.artists.getEnumerator()) {
		if(!$artist -or $x.key -like $artist) {
			$x.value | where-object {
				(!$album -or $_.album -like $album) -and
				(!$title -or $_.title -like $title)
			}
		}
	}
}

function Sync-Mpd {
	$script:MPD.reload()
}

function Play-Track {
	[CmdletBinding(DefaultParameterSetName = "object")]
	param (
		[parameter(
			ParameterSetName = "object",
			HelpMessage = "Track to play",
			Position = 0,
			ValueFromPipeline,
			Mandatory
		)]
		[Track[]] $Track,

		[Parameter(
			ParameterSetName = "query",
			HelpMessage = "The title of the track",
			Position = 0
		)]
		[string] $Title,
		[Parameter(
			ParameterSetName = "query",
			HelpMessage = "The name of the artist"
		)]
		[string] $Artist,
		[Parameter(
			ParameterSetName = "query",
			HelpMessage = "The name of the album"
		)]
		[string] $Album,

		[Parameter(ParameterSetName = "object", HelpMessage = "Append instead of replacing")]
		[Parameter(ParameterSetName = "query", HelpMessage = "Append instead of replacing")]
		[switch] $Queue
	)

	begin {
		$tracks = [List[Track]]::new()
		if($PSCmdlet.ParameterSetName -ceq "query") {
			$tracks = Get-Track -artist:$artist -album:$album -title:$title
		}
	}
	process {
		if($track) {
			[void] $tracks.AddRange($track)
		}
	}
	end {
		if($tracks) {
			if(!$queue) {
				script::mpc clear
			}
			$tracks.file | mpc.exe -q add
			if(!$queue) {
				script::mpc play
			}

			if($queue -and $tracks.count -eq 1) {
				write-information "Added $($tracks[0]) to the queue"
			} elseif($queue) {
				write-information "Added $($tracks.count) tracks to the queue"
			} elseif($tracks.count -eq 1) {
				write-information "Playing $($tracks[0])"
			} else {
				write-information "Playing $($tracks.count) tracks"
			}
		} else {
			write-warning "No tracks found"
		}
	}
}
