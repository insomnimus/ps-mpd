using namespace System.Collections.Generic

$script:MPD = [Mpd]::new()
$script:fmt = "%artist%`u{1}%title%`u{1}%album%`u{1}%time%`u{1}%track%`u{1}%file%"
$script:ModTimes = @{}

$ExecutionContext.SessionState.Module.OnRemove += {
	$script:MPD.artists.clear()
	$script:MPD.playlists.clear()
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
		return $this.ToString($false)
	}

	[string] ToString([bool] $full) {
		$art = if($this.artist) { $this.artist } else { "?" }
		$alb = if($this.album) { $this.album } else { "?" }
		$tit = if($this.title) { $this.title } else { $this.file }
		if(!$full) {
			return "$tit [$alb] by $art"
		}
		$dur = if($this.duration.TotalHours -ge 1) { $this.duration.ToString() } else { "{0}:{1}" -f $this.duration.minutes, $this.duration.seconds }
		return "$tit [$alb] by $art [$dur]"
	}

	[void] Play() {
		script:Play-Track $this
	}

	[void] Queue() {
		script:Play-Track -queue $this
	}

	[bool] Matches([string[]] $title, [string] $artist, [string] $album) {
		if((!$album -or $this.album -like $album) -and (!$artist -or $this.artist -like $artist)) {
			if(!$title) {
				return $true
			}
			if(!$this.title) {
				return $false
			}
			foreach($t in $title) {
				if($this.title -like $t) {
					return $true
				}
			}
		}
		return $false
	}
}

class MPDStatus {
	[Track] $Track
	[int] $Volume

	[string] ToString() {
		return "$($this.track)`volume $($this.volume)%"
	}

	MPDStatus([Track] $track, [int32] $vol) {
		$this.track = $track
		$this.volume = $vol
	}
}

class Playlist {
	[string] $Path
	[string] $Name
	[List[Track]] $Tracks

	[string] ToString() {
		return $this.Name
	}

	[void] Save([bool] $force) {
		$err = $null
		if(!$force) {
			$f = get-item -lp $this.path -ea ignore -ev err
			if($err) { throw $err.message }
			if($f.lastWriteTimeUtc -gt $script:ModTimes[$this.path]) {
				throw "the playlist file has been modified since load and the Force parameter is not set"
			}
		}

		$this.tracks.file | join-string -separator "`n" | out-file -lp $this.path -encoding utf8 -noNewLine -ea ignore -ev err
		if($err) {
			throw $err.message
		}
		$f = get-item -ea ignore -lp $this.path
		if($f) {
			$script:ModTimes[$this.path] = $f.LastWriteTimeUTC
		}
	}
}

class Mpd {
	[string] $playlistsDir
	[SortedDictionary[string, [List[Track]]]]
	$artists = [SortedDictionary[string, [List[Track]]]]::new([StringComparer]::InvariantCultureIgnoreCase)
	[SortedDictionary[string, Playlist]]
	$Playlists = [SortedDictionary[string, Playlist]]::new([StringComparer]::InvariantCultureIgnoreCase)

	[void] reload() {
		$this.artists.clear()
		$this.playlists.clear()

		$tracks = [Dictionary[string, Track]]::new()
		$loadPlaylists = $this.playlistsDir -and (test-path -type container -lp $this.playlistsDir)

		foreach($val in script::mpc -f $script:fmt listall) {
			$song = [Track]::parse($val)
			if(!$song) {
				continue
			}
			if($loadPlaylists) {
				$tracks[$song.file] = $song
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

		# load playlists
		if(!$this.playlistsDir) {
			return
		} elseif(!$loadPlaylists) {
			write-warning "the playlists directory does not exist or is not a directory ($($this.playlistsDir))"
			return
		}

		foreach($pl in get-childitem -recurse -lp $this.playlistsDir -file -filter "*.m3u") {
			$script:ModTimes[$pl.fullname] = $pl.LastWriteTimeUTC
			$plist = [Playlist] @{
				Name = $pl.basename
				Path = $pl.fullname
				Tracks = [List[Track]]::new()
			}

			foreach($s in get-content -lp $pl.fullname -ea continue) {
				$s = $s.trim()
				if($s.startswith("#")) {
					continue
				}
				[Track] $t = $null
				if($tracks.TryGetValue($s, [ref] $t)) {
					[void] $plist.tracks.add($t)
				}
			}

			[void] $this.playlists.Add($plist.name, $plist)
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
	mpc -q $command
	if($LastExitCode -ne 0) {
		write-error "mpc process exited with $lastExitCode"
	}
}

function Play-Next {
	script::mpc next
	Get-MPDStatus
}

function Play-Previous {
	script::mpc prev
	Get-MPDStatus
}

function Sync-Mpd {
	[CmdletBinding()]
	param (
		[Parameter(HelpMessage = "Absolute path of the playlists directory")]
		[ValidateScript({ test-path -type container $_ })]
		[string] $PlaylistsDir,
		[Parameter(HelpMessage = "Update mpd before loading songs")]
		[switch] $UpdateMPD
	)
	if($UpdateMPD) {
		mpc -q update -w
	}

	if($playlistsDir) {
		$script:MPD.playlistsDir = $playlistsDir
	}
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

		[Parameter(ParameterSetName = "current-track", HelpMessage = "Get the currently playing song")]
		[switch]$Playing,
		[Parameter(HelpMessage = "Get the playing queue", ParameterSetName = "current-queue")]
		[switch] $Queue
	)

	if($playing) {
		script::mpc -f $script:fmt current | foreach-object { [Track]::parse($_) }
		return
	} elseif($queue) {
		script::mpc -f $fmt playlist | foreach-object { [Track]::Parse($_) }
		return
	}

	foreach($x in $script:MPD.artists.getEnumerator()) {
		if(!$artist -or $x.key -like $artist -or $x.key -eq $artist) {
			$x.value | script:Select-Track -title:$title -album:$album
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
			$tracks.file | mpc -q add
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

function Get-MPDStatus {
	[CmdletBinding()]
	[OutputType([MPDStatus])]
	param ()

	$vol = script::mpc volume | join-string { $_ -replace "^volume\:\s*", "" }
	if($vol -eq "n/a") {
		$vol = "-1"
	}
	$t = script:Get-Track -playing
	[MPDStatus]::new($t, $vol.trim("%"))
}

function Get-Playlist {
	[CmdletBinding()]
	[OutputType([Playlist])]
	param (
		[Parameter(Position = 0, HelpMessage = "Name of the playlist")]
		[Alias("Playlist")]
		[string[]] $Name
	)

	foreach($x in $script:MPD.playlists.GetEnumerator()) {
		if(!$name) {
			$x.value
			continue
		}
		foreach($n in $name) {
			if($x.key -like $n) {
				$x.value
				continue
			}
		}
	}
}

function Play-Playlist {
	[CmdletBinding(DefaultParameterSetName = "query")]
	param (
		[Parameter(
			ParameterSetName = "query",
			HelpMessage = "Name of the playlist",
			Mandatory,
			Position = 0
		)]
		[string] $Name,
		[Parameter(
			HelpMessage = "The track name to start playing from",
			Position = 1
		)]
		[string] $Track,

		[Parameter(
			HelpMessage = "The input playlist object",
			ParameterSetName = "object",
			ValueFromPipeLine,
			Mandatory,
			Position = 0
		)]
		[Playlist] $InputObject,

		[Parameter(ParameterSetName = "query", HelpMessage = "Add the tracks at the end of the queue")]
		[Parameter(ParameterSetName = "object", HelpMessage = "Add the tracks at the end of the queue")]
		[switch] $Queue
	)

	begin {}
	process {}
	end {
		$pls = if($inputObject) {
			$inputObject
		} else {
			script:Get-Playlist -Name:$name | select-object -first 1
		}
		if(!$pls) {
			write-warning "No playlist found"
			return
		}

		$name = $pls.name
		if($pls.tracks.count -eq 0) {
			write-warning "The playlist $name has no tracks"
			return
		}

		if(!$track) {
			$index = ""
		} else {
			$index = -1
			for($i = 0; $i -lt $pls.tracks.count; $i++) {
				if($pls.tracks[$i].title -eq $track -or $pls.tracks[$i].title -like $track) {
					$index = $i + 1
					break
				}
			}
			if($index -le 0) {
				write-error "No track title in $name matched the given pattern"
				return
			}
		}

		$ntracks = script::plural $pls.tracks.count "track"

		if($queue) {
			$pls.tracks.file | mpc -q add
			if($lastExitCode -eq 0) {
				write-information "Added $ntracks from $name to the queue"
			}
		} else {
			script::mpc -ea stop clear
			$pls.tracks.file | mpc -q add
			if($lastExitCode -eq 0) {
				script::mpc -ea stop play $index
				if($index) {
					write-information "Playing $($pls.tracks[$index - 1]) from $name ($ntracks total)"
				}
			}
		}
	}
}

function Save-Playlist {
	[CmdletBinding(DefaultParameterSetName = "query")]
	param (
		[Parameter(
			Position = 0,
			ParameterSetName = "query",
			Mandatory,
			HelpMessage = "Name of the playlist to save"
		)]
		[string[]] $Name,

		[Parameter(
			Position = 0,
			ParameterSetName = "object",
			Mandatory,
			HelpMessage = "The Playlist object"
		)]
		[Playlist[]] $InputObject
	)

	begin {
		[List[Playlist]] $playlists = [List[Playlist]]::new()
		if($PSCmdlet.ParameterSetName -eq "query") {
			$playlists = Get-Playlist -Name:$Name
		}
	}

	process {
		if($InputObject) {
			[void] $playlists.AddRange($InputObject)
		}
	}

	end {
		if($playlists.count -eq 0) {
			write-warning "No playlist matched the given criteria"
			return
		}
		foreach($p in $playlists) {
			try {
				$p.Save($true)
				write-information "Saved playlist $p"
			} catch {
				write-error "error saving $($p.name) to $($p.path): $_"
			}
		}
	}
}

function Save-Track {
	[CmdletBinding(DefaultParameterSetName = "object")]
	param (
		[Parameter(
			Position = 0,
			Mandatory,
			HelpMessage = "The playlist object or name of the playlist to save the track to"
		)]
		[Object[]] $Playlist,

		[Parameter(
			HelpMessage = "Name of the track to save",
			Position = 1,
			ParameterSetName = "query"
		)]
		[string[]] $title,
		[Parameter(HelpMessage = "The name of the artist", ParameterSetName = "query")]
		[string] $Artist,
		[Parameter(ParameterSetName = "query", HelpMessage = "Name of the album")]
		[string] $Album,

		[Parameter(
			Mandatory,
			Position = 1,
			ValueFromPipeline,
			ValueFromPipelineByPropertyName,
			HelpMessage = "The input Track object",
			ParameterSetName = "object"
		)]
		[Alias("Track")]
		[Track[]] $InputObject,

		[Parameter(HelpMessage = "Do not save the playlist to disk")]
		[Alias("NS")]
		[switch] $NoSave,
		[Parameter(HelpMessage = "Allow adding tracks even if they are alreawdy in the playlist")]
		[switch] $AllowDuplicates,
		[Parameter(HelpMessage = "Overwrite any externally made changes since last sync")]
		[switch] $Force
	)

	begin {
		[List[Playlist]] $playlists = [List[Playlist]]::new()
		foreach($p in $playlist) {
			if($p -is [Playlist]) {
				[void] $playlists.Add($p)
			} elseif($p) {
				$pls = script:get-playlist -name:"$p"
				if($pls.count -eq 1) {
					[void] $playlists.Add($pls)
				} elseif($pls.count -eq 0) {
					write-error "No playlist matched the criteria: 4p"
					return
				} else {
					[void] $playlists.AddRange($pls)
				}
			}

			$playlists = $playlists | select-object -unique

			if(!$playlists) {
				write-warning "No playlist found"
				return
			}

			[list[Track]] $tracks = [List[Track]]::new()
			if($PSCmdlet.ParameterSetName -eq "query") {
				$tracks = script:Get-Track -title:$title -artist:$artist -album:$album
				if($tracks.count -eq 0) {
					write-error "No tracks matched given criteria"
					return
				}
			}
		}
	}

	process {
		if($inputObject) {
			[void] $tracks.AddRange($inputObject)
		}
	}

	end {
		$tracks = $tracks | select -unique

		switch($tracks.count) {
			0 { write-error "No tracks given"; return }
			1 {
				$t = $tracks[0]
				foreach($p in $playlists) {
					$save = $force
					if(!$allowDuplicates -and $p.tracks.exists({ $t.file -eq $args[0].file })) {
						write-information "$t is already in $p"
						$save = $force
					} else {
						$save = !$NoSave
						[void] $p.tracks.add($t)
						write-information "Added $t to $p"
					}

					if(!$NoSave -and $save) {
						try {
							$p.save($force)
							write-information "saved $p"
						} catch {
							write-error "Error saving $p to $($p.path): $_"
						}
					}
				}
				break
			}

			default {
				foreach($p in $playlists) {
					$added = 0

					if($allowDuplicates) {
						[void] $p.tracks.AddRange($tracks)
						$added = $tracks.count
					} else {
						foreach($t in $tracks) {
							if($p.tracks.exists({ $t.file -eq $args[0].file })) {
								write-information "$t is already in $p"
							} else {
								[void] $p.tracks.add($t)
								$added++
							}
						}
					}

					$added = script::plural $added "track"
					write-information "Added $added to $p"
					if(!$noSave) {
						try {
							$p.save($force)
							write-information "Saved $p"
						} catch {
							write-error "error saving $($p.name) to $($p.path): $_"
						}
					}
				}
			}
		}
	}
}

function Remove-Track {
	[CmdletBinding(DefaultParameterSetName = "object")]
	param (
		[Parameter(
			Position = 0,
			Mandatory,
			HelpMessage = "The playlist object or name of the playlist"
		)]
		[Object[]] $Playlist,

		[Parameter(
			HelpMessage = "Name of the track to remove",
			Position = 1,
			ParameterSetName = "query"
		)]
		[string[]] $title,
		[Parameter(HelpMessage = "The name of the artist", ParameterSetName = "query")]
		[string] $Artist,
		[Parameter(ParameterSetName = "query", HelpMessage = "Name of the album")]
		[string] $Album,

		[Parameter(
			Mandatory,
			Position = 1,
			ValueFromPipeline,
			ValueFromPipelineByPropertyName,
			HelpMessage = "The input Track object",
			ParameterSetName = "object"
		)]
		[Alias("Track")]
		[Track[]] $InputObject,

		[Parameter(HelpMessage = "Do not save the playlist to disk")]
		[switch] $NoSave,
		[Parameter(HelpMessage = "Force overwriting any externally made changes since last sync")]
		[switch] $Force
	)

	begin {
		[List[Playlist]] $playlists = [List[Playlist]]::new()
		foreach($p in $playlist) {
			if($p -is [Playlist]) {
				[void] $playlists.Add($p)
			} elseif($p) {
				$pls = script:get-playlist -name:"$p"
				if($pls.count -eq 1) {
					[void] $playlists.Add($pls)
				} elseif($pls.count -eq 0) {
					write-error "No playlist matched the criteria: 4p"
					return
				} else {
					[void] $playlists.AddRange($pls)
				}
			}

			$playlists = $playlists | select-object -unique

			if(!$playlists) {
				write-warning "No playlist found"
				return
			}
		}

		[List[Track]] $tracks = [List[Track]]::new()
	}

	process {
		if($inputObject) {
			[void] $tracks.AddRange($inputObject)
		}
	}

	end {
		if($PSCmdlet.ParameterSetName -eq "object") {
			foreach($p in $playlists) {
				$n = $p.tracks.removeAll({
						foreach($t in $tracks) {
							if($t.file -eq $args[0].file) {
								return $true
							}
						}
						$false
					})

				if($n -eq 0 -and -not $force) {
					continue
				}
				if($n -ne 0 -and $tracks.count -eq 1) {
					write-information "Removed $($tracks[0]) from $p"
				} elseif($n -ne 0) {
					$removed = script::quote $n "track"
					write-information "Removed $removed from $p"
				}

				if(!$NoSave -and ($force -or $n -gt 0)) {
					try {
						$p.save($force)
						write-information "Saved $p to disk"
					} catch {
						write-error "Error saving $p to disk: $_"
					}
				}
			}
			return
		}

		foreach($p in $playlists) {
			$del = [List[Track]]::new()
			[void] $p.tracks.removeAll({
					param($t)
					if((!$artist -or $t.artist -like $artist) -and (!$album -or $t.album -like $album)) {
						if(!$title) {
							[void] $del.Add($t)
							return $true
						}
						foreach($s in $title) {
							if($t.title -like $s) {
								[void] $del.add($t)
								return $true
							}
						}
					}
					$false
				})

			$del = $del | select-object -unique

			if($del.count -eq 0) {
				write-warning "No track to remove found in $p"
				if(!$force -or $noSave) { continue }
			} elseif($del.count -eq 1) {
				write-information "Removed $($del[0]) from $p"
			} else {
				$removed = script::plural $del.count "track"
				write-information "Removed $removed from $p"
			}

			if(!$NoSave) {
				try {
					$p.save($force)
					write-information "Saved $p"
				} catch {
					write-error "Error saving $p to $($p.path): $_"
				}
			}
		}
	}
}

function Seek-Queue {
	[CmdletBinding()]
	param (
		[Parameter(
			ValueFromPipelineByPropertyName,
			Position = 0,
			HelpMessage = "The title of a song from the current queue to seek to"
		)]
		[string] $Title,
		[Parameter(
			HelpMessage = "The name of the artist",
			ValueFromPipelineByPropertyName
		)]
		[string] $Artist,
		[Parameter(
			HelpMessage = "Name of the album",
			ValueFromPipelineByPropertyName
		)]
		[string] $Album
	)

	begin {
		if(!$title -and !$artist -and !$album) {
			write-error "You must specify at least one of 'Title', 'Artist' or 'Album'"
			return
		}
	}

	process {}

	end {
		$i = 1
		foreach($t in script:get-track -queue) {
			if($t.matches($title, $artist, $album)) {
				script::mpc play $i
				if($?) {
					write-information "Playing track $i ($t)"
				}
				return
			}
			$i++
		}

		write-error "No track in the queue matched the given criteria"
	}
}

function Select-Track {
	[CmdletBinding()]
	[OutputType([Track])]
	param (
		[Parameter(Position = 0, HelpMessage = "The track title to match")]
		[string[]] $Title,
		[Parameter(HelpMessage = "The Artist name to match")]
		[string[]] $Artist,
		[Parameter(HelpMessage = "The album name to match")]
		[string[]] $Album,

		[Parameter(HelpMessage = "Select first N tracks that match the criteria")]
		[uint] $First,

		[Parameter(
			ValueFromPipeLine,
			ValueFromPipeLineByPropertyName,
			HelpMessage = "The input Track object"
		)]
		[Alias("Track")]
		[Track[]] $InputObject
	)

	begin {
		[uint] $n = 0
		$takeFirst = $psBoundParameters.ContainsKey("First")
	}
	process {
		foreach($t in $inputObject) {
			if($takeFirst -and $n -ge $first) {
				return
			}
			if($artist) {
				if(!$t.artist) { continue }
				$found = $false
				foreach($a in $artist) {
					if($t.artist -like $a -or $t.artist -eq $a) {
						$found = $true
						break
					}
				}
				if(!$found) { continue }
			}

			if($album) {
				if(!$t.album) { continue }
				$found = $false
				foreach($a in $album) {
					if($t.album -like $a -or $t.album -eq $a) {
						$found = $true
						break
					}
				}
				if(!$found) { continue }
			}

			if($title) {
				if(!$t.title) { continue }
				$found = $false
				foreach($s in $title) {
					if($t.title -like $s -or $t.title -eq $s) {
						$found = $true
						break
					}
				}
				if(!$found) { continue }
			}

			write-output $t
			$n++
		}
	}
}

function Save-Playing {
	[CmdletBinding(DefaultParameterSetName = "query")]
	param (
		[Parameter(
			Position = 0,
			Mandatory,
			HelpMessage = "The name of the playlist to save to",
			ParameterSetName = "query"
		)]
		[string[]] $Name,

		[Parameter(
			Position = 0,
			Mandatory,
			ValueFromPipeline,
			HelpMessage = "The input Playlist object",
			ParameterSetName = "object"
		)]
		[Playlist[]] $InputObject,

		[Parameter(HelpMessage = "Allow adding duplicates")]
		[switch] $AllowDuplicates,
		[Parameter(HelpMessage = "Force overwriting any externally made changes since last sync")]
		[switch] $Force
	)

	begin {
		$t = get-track -playing
		if(!$t) {
			write-error "Not playing anything"
			return
		}
		[List[Playlist]] $playlists = @()
		if($PSCmdlet.ParameterSetName -eq "query") {
			$playlists = script:get-playlist -name:$name
		}
	}

	process {
		if($InputObject) {
			[void] $playlists.AddRange($InputObject)
		}
	}

	end {
		$playlists = $playlists | select-object -unique
		if($playlists.count -eq 0) {
			write-error "No playlist matched the given criteria"
			return
		}
		$t | save-track $playlists -force:$force
	}
}
