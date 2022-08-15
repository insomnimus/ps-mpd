filter :quote {
	if($_ -match "[\s`'`"``\(\)\{\}]") {
		$s = $_.replace("'", "''")
		"'$s'"
	} else {
		$_
	}
}

function :normalize-arg {
	param(
		[parameter(mandatory, position = 0)]
		[allowEmptyString()]
		[string] $buf
	)

	if($buf.startswith("'")) {
		if($buf.length -gt 1 -and $buf.endsWith("'")) {
			$buf = $buf.substring(1, $buf.length - 2)
		} else {
			$buf = $buf.substring(1)
		}
	} elseif($buf.startswith('"')) {
		if($buf.length -gt 1 -and $buf.endsWith('"')) {
			$buf = $buf.substring(1, $buf.length - 2)
		} else {
			$buf = $buf.substring(1)
		}
	}

	if($null -eq $buf) { $buf = "" }

	if(!$buf.endswith("*")) {
		$buf += "*"
	}

	$buf
}

"Artist", "Album", "Title" | foreach-object {
	Register-ArgumentCompleter -CommandName Get-Track, Play-Track, Save-Track -ParameterName $_ -ScriptBlock {
		param($_a, $paramName, $buf, $_d, $params)

		$params[$paramName] = script::normalize-arg $buf
		$params = @{
			Artist = $params["Artist"]
			Album = $params["Album"]
			Title = $params["Title"]
		}

		Get-Track @params `
		| select-object -ExpandProperty $paramName `
		| where-object { $_ } `
		| sort-object -unique `
		| script::quote
	}
}

"Name", "Artist" | foreach-object {
	Register-ArgumentCompleter -CommandName Get-Album, Play-Album -ParameterName $_ -ScriptBlock {
		param($_a, $paramName, $buf, $_d, $params)

		$buf = script::normalize-arg $buf
		$field = if($paramName -eq "Name") { "Album" } else { $paramName }
		$params[$field] = $buf
		$params = @{
			Artist = $params["Artist"]
			Album = $params["Album"]
		}

		Get-Track @params `
		| select-object -ExpandProperty $field `
		| where-object { $_ } `
		| sort-object -unique `
		| script::quote
	}
}

Register-ArgumentCompleter -CommandName Get-Artist, Play-Artist -ParameterName Name -ScriptBlock {
	param($_a, $_b, $buf, $_d, $_params)

	$buf = script::normalize-arg $buf
	$script:MPD.artists.Keys `
	| where-object { $_ -and $_ -like $buf } `
	| script::quote
}

$completePlaylist = {
	param($_a, $_b, $buf, $_d, $_params)
	$buf = script::normalize-arg $buf

	$script:MPD.playlists.keys `
	| where-object { $_ -and $_ -like $buf } `
	| script::quote
}

Register-ArgumentCompleter -CommandName Get-Playlist, Play-Playlist -ParameterName Name -ScriptBlock $completePlaylist
Register-ArgumentCompleter -CommandName Save-Track, Remove-Track -ParameterName Playlist -ScriptBlock $completePlaylist
Register-ArgumentCompleter -CommandName Save-Playing -ParameterName Name -ScriptBlock $completePlaylist

Register-ArgumentCompleter -CommandName Play-Playlist -ParameterName Track -ScriptBlock {
	param($_a, $_b, $buf, $_d, $params)

	$pl = $params["Name"]
	if(!$pl) {
		return
	}

	$buf = script::normalize-arg $buf
	foreach($x in $script:MPD.playlists.values) {
		if($x.name -like $pl) {
			$x.tracks.title `
			| where-object { $_ -and $_ -like $buf } `
			| script::quote

			return
		}
	}
}

"Album", "Artist", "Title" | foreach-object {
	Register-ArgumentCompleter -CommandName Remove-Track -ParameterName $_ -ScriptBlock {
		param($_a, $paramName, $buf, $_d, $params)

		if(!$params["Playlist"]) {
			return
		}

		$params[$paramName] = script::normalize-arg $buf
		[string[]] $title = $params["Title"]
		$artist = $params["Artist"]
		$album = $params["Album"]

		<#
		$filter = {
			param($t)
			if((!$artist -or $t.artist -like $artist) -and (!$album -or $t.album -like $album)) {
				if(!$title) {
					return $true
				}
				foreach($s in $title) {
					if($t.title -like $s) {
						return $true
					}
				}
			}
			return $false
		}
#>

		foreach($p in $params["Playlist"]) {
			if($p -is [Playlist]) {
				# $p.tracks | where-object { $filter.invoke($_) } | select-object -expandProperty $paramName | script::quote
				$p.tracks | script:select-track -title:$title -artist:$artist -album:$album | select-object -expandProperty $paramName | where-object { $_ } | script::quote
			} else {
				foreach($p in Get-Playlist -Name:"$p") {
					# $p.tracks | where-object { $filter.invoke($_) } | select-object -expandProperty $paramName | script::quote
					$p.tracks | script:select-track -title:$title -artist:$artist -album:$album | select-object -expandProperty $paramName | where-object { $_ } | script::quote
				}
			}
		}
	}
}

"Album", "Artist", "Title" | foreach-object {
	Register-ArgumentCompleter -CommandName Seek-Queue -ParameterName $_ -ScriptBlock {
		param($_a, $paramName, $buf, $_d, $params)

		$params[$paramName] = script::normalize-arg $buf
		$title = $params["Title"]
		$artist = $params["Artist"]
		$album = $params["Album"]

		script:Get-Track -queue `
		| script:select-track -title:$title -artist:$artist -album:$album `
		| select-object -expandProperty $paramName `
		| where-object { $_ } `
		| script::quote
	}
}
