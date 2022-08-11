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
		$buf = $buf.trim("'")
	} elseif($buf.startswith('"')) {
		$buf = $buf.trim('"')
	}
	if(!$buf.endswith("*")) {
		$buf += "*"
	}

	$buf
}

"Artist", "Album", "Title" | foreach-object {
	Register-ArgumentCompleter -CommandName Get-Track, Play-Track, Save-Track -ParameterName $_ -ScriptBlock {
		param($_a, $paramName, $buf, $_d, $params)

		$buf = script::normalize-arg $buf
		$params[$paramName] = $buf
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
Register-ArgumentCompleter -CommandName Get-Playlist, Save-Track -ParameterName Playlist -ScriptBlock $completePlaylist

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
