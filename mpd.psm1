using namespace System.Collections.Generic

$script:MPD = [Mpd]::new()
$script:fmt = "%artist%`u{1}%title%`u{1}%album%`u{1}%time%`u{1}%track%`u{1}%file%"

$ExecutionContext.SessionState.Module.OnRemove += {
	$script:MPD.artists.clear()
	$script:MPD = $null
}

class Track {
	[string] $Title
	[string] $Artist
	[string] $Album
	[TimeSpan] $Duration
	[int] $TrackNo
	[string] $File

	static [Track] Parse([string]$s) {
		$c = [char]1
		$_artist, $_title, $_album, $_duration, $_trackno, $_file = $s.split($c) | foreach-object { $_.trim() }
		if($null -eq $_file) {
			return $null
		}
		$split = "$_duration".split(":")
		$_duration = switch($split.count) {
			1 { [TimeSpan]::new(0, 0, $split[0]); break }
			2 { [TimeSpan]::new(0, $split[0], $split[1]); break }
			3 { [TimeSpan]::new($split[0], $split[1], $split[2]); break }
			4 {
				$hours = ([int] $split[0]) * 24 + $split[1]
				[TimeSpan]::new($hours, $split[2], $split[3])
				break
			}
			default { [TimeSpan]::Zero }
		}
		$n = 0
		if(-not [int]::TryParse($_trackno, [ref] $n)) {
			$n = -1
		}

		$track = [Track] @{
			Title = $_title
			Artist = $_artist
			Album = $_Album
			Duration = $_duration
			TrackNo = $n
			File = $_file
		}
		return $track
	}

	[string] ToString() {
		$art = if($this.artist) { $this.artist } else { "?" }
		$alb = if($this.album) { $this.album } else { "?" }
		$tit = if($this.title) { $this.title } else { $this.file }
		$dur = if($this.duration.TotalHours -ge 1) { $this.duration.ToString() } else { "{0}:{1}" -f $this.duration.minutes, $this.duration.seconds }
		return "$tit [$alb] by $art [$dur]"
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

class Album {
	[string] $Name
	[string] $Artist
	[Track[]] $Tracks

	[string] ToString() {
		$_name = if($this.name) { $this.name } else { "?" }
		$_artist = if($this.artist) { $this.artist } else { "?" }
		return "$_name by $_artist"
	}
}

class Artist {
	[string] $Name
	[Album[]] $Albums
}

function :plural {
	param(
		[int] $n,
		[string] $s
	)
	if($n -eq 1) {
		"1 $s"
	} else {
		"$n ${s}s"
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

function Sync-Mpd {
	$script:MPD.reload()
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
		[string[]] $Title,
		[parameter(
			ParameterSetName = "query",
			HelpMessage = "Name of the artist"
		)]
		[string] $Artist,
		[parameter(
			ParameterSetName = "query",
			HelpMessage = "Name of the album"
		)]
		[string] $Album,

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
				if($album -and $_.album -notlike $album) { return $false }
				if(!$title) { return $true }
				foreach($t in $title) {
					if($_.title -like $t) { return $true }
				}
				$false
			}
		}
	}
}

function Play-Track {
	[CmdletBinding(DefaultParameterSetName = "object")]
	param (
		[Parameter(
			ParameterSetName = "query",
			HelpMessage = "The title of the track",
			Position = 0
		)]
		[string[]] $Title,
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

		[parameter(
			ParameterSetName = "object",
			HelpMessage = "Track to play",
			Position = 0,
			ValueFromPipeline,
			Mandatory
		)]
		[Track[]] $Track,

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

function Get-Album {
	[CmdletBinding()]
	[OutputType([Album])]
	param (
		[Parameter(
			Position = 0,
			HelpMessage = "Name of the album"
		)]
		[string[]] $Name,
		[Parameter(HelpMessage = "Name of the artist")]
		[string] $Artist
	)

	$res = if($name) {
		foreach($n in $name) {
			script:get-track -artist:$artist -album:$n
		}
	} else {
		script:get-track -artist:$artist
	}

	$res `
		# TODO: make it select unique by file name
	| select-object -unique `
	| group-object -property Album `
	| foreach-object {
		[Album] @{
			Name = $_.Name
			artist = $_.group[0].artist
			tracks = ($_.group | sort-object -stable -property trackNo)
		}
	}
}

function Play-Album {
	[CmdletBinding(DefaultParameterSetName = "query")]
	param (
		[Parameter(HelpMessage = "Name of the artist", ParameterSetName = "query")]
		[string] $Artist,
		[Parameter(Position = 0, HelpMessage = "Name of the album", ParameterSetName = "query")]
		[string[]] $Name,

		[Parameter(
			ValueFromPipeLine,
			Mandatory,
			Position = 0,
			HelpMessage = "The Album object",
			ParameterSetName = "object"
		)]
		[Album[]] $InputObject,

		[Parameter(ParameterSetName = "object", HelpMessage = "Add the album at the end of the queue")]
		[Parameter(ParameterSetName = "query", HelpMessage = "Add the album at the end of the queue")]
		[switch] $Queue
	)

	begin {
		[List[Album]] $albums = @()
		if($PSCmdlet.ParameterSetName -eq "query") {
			$albums = script:Get-Album -name:$name -artist:$artist
		}
	}
	process {
		if($inputObject) {
			[void] $albums.AddRange($inputObject)
		}
	}
	end {
		$ntracks = $albums.tracks | measure-object | select-object -expandProperty Count
		if($ntracks -eq 0) {
			write-warning "No tracks found"
			return
		} elseif($ntracks -eq 1) {
			$ntracks = "1 track"
		} else {
			$ntracks = "$ntracks tracks"
		}

		switch($albums.count) {
			1 {
				$albums[0].tracks | script:play-track -queue:$queue -infa ignore
				if($queue) {
					write-information "Added $($albums[0]) to the queue ($ntracks)"
				} else {
					write-information "Playing $($albums[0]) ($ntracks)"
				}
				break
			}
			0 {
				write-warning "No albums found"
				return
			}
			default {
				$albums.tracks | script:Play-Track -queue:$queue -infa ignore
				$len = $albums.count
				if($queue) {
					write-information "Added $len albums to the queue ($ntracks)"
				} else {
					write-information "Playing $len albums ($ntracks)"
				}
			}
		}
	}
}

function Get-Artist {
	[CmdletBinding()]
	[OutputType([Artist])]
	param (
		[Parameter(Position = 0, HelpMessage = "Name of the artist")]
		[string[]] $Name
	)
	if(!$name) {
		$name = [string[]] @("*")
	}

	foreach($n in $name) {
		foreach($x in $script:MPD.artists.GetEnumerator()) {
			if($x.key -like $n) {
				$albums = $x.value | group-object -property Album | foreach-object {
					[Album] @{
						Name = $_.Name
						Tracks = ($_.group | sort-object -stable -property TrackNo)
						Artist = $x.Key
					}
				}

				[Artist] @{ Name = $x.key; Albums = $albums }
			}
		}
	}
}

function Play-Artist {
	[CmdletBinding(DefaultParameterSetName = "query")]
	param (
		[Parameter(Mandatory, Position = 0, HelpMessage = "Name of the artist", ParameterSetName = "query")]
		[string[]] $Name,

		[Parameter(Mandatory, Position = 0, ValueFromPipeline, ParameterSetName = "object", HelpMessage = "The input Artist object")]
		[Artist[]] $InputObject,

		[Parameter(ParameterSetName = "object", HelpMessage = "Add the artists tracks at the end of the queue")]
		[Parameter(ParameterSetName = "query", HelpMessage = "Add the artists tracks at the end of the queue")]
		[switch] $Queue
	)

	begin {
		[List[Artist]] $artists = @()
		if($PSCmdlet.ParameterSetName -eq "query") {
			$artists = script:Get-Artist -name:$name
		}
	}
	process {
		if($inputObject) {
			[void] $artists.AddRange($inputObject)
		}
	}
	end {
		$nAlbs = $artists.albums.count
		$ntracks = $artists.albums.tracks.count
		if($ntracks -eq 0) {
			write-warning "No tracks found"
			return
		}
		$ntracks = script::plural $ntracks "track"
		$nalbs = script::plural $nalbs "album"

		switch($artists.count) {
			0 { write-warning "No artist found"; return }
			1 {
				$artists[0].albums.tracks | script:play-track -queue:$queue -infa ignore
				$name = if($artists[0].name) { $artists[0].name } else { "?" }
				if($queue) {
					write-information "Added $name ($nalbs, $ntracks) to the queue"
				} else {
					write-information "Playing $name ($nalbs, $ntracks)"
				}
				break
			}
			default {
				$artists.albums.tracks | script:play-track -infa ignore -queue:$queue
				if($queue) {
					write-information "Added $_ artists ($nalbs, $ntracks) to the queue"
				} else {
					write-information "Playing $_ artists ($nalbs, $ntracks)"
				}
			}
		}
	}
}

set-alias stra get-track
set-alias ptra play-track
set-alias salb Get-Album
set-alias palb Play-Album
set-alias sart Get-Artist
set-alias part Play-Artist
