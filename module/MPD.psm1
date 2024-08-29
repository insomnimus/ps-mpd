foreach($s in "Artist", "Album", "Title") {
	Register-ArgumentCompleter -CommandName Get-Track, Play-Track, Save-Track -ParameterName $s -ScriptBlock {
		param($_a, $paramName, $word, $_d, $params)

		$params[$paramName] = @()
		$params = @{
			Artist = $params["Artist"]
			Album = $params["Album"]
			Title = $params["Title"]
			PropertyName = $paramName
		}

		Complete-Track -word:$word @params
	}
}

foreach($s in "Title", "Artist") {
	Register-ArgumentCompleter -CommandName Get-Album, Play-Album -ParameterName $s -ScriptBlock {
		param($_a, $paramName, $word, $_d, $params)

		$params = @{
			Artist = $params["Artist"]
			Title = $params["Title"]
			PropertyName = $paramName
		}

		Complete-Album -word:$word @params
	}
}

Register-ArgumentCompleter -CommandName Get-Artist, Play-Artist -ParameterName Name -ScriptBlock {
	param($_a, $_b, $word)

	Complete-Artist -word:$word
}

$completePlaylist = {
	param($_a, $_b, $word)

	Complete-Playlist -word:$word
}

Register-ArgumentCompleter -CommandName Get-Playlist, Play-Playlist -ParameterName Name -ScriptBlock $completePlaylist
Register-ArgumentCompleter -CommandName Save-Playing, Save-Track, Remove-Track -ParameterName Playlist -ScriptBlock $completePlaylist

Register-ArgumentCompleter -CommandName Play-Playlist -ParameterName Seek -ScriptBlock {
	param($_a, $_b, $word, $_d, $params)

	$pl = $params["Name"]

	Complete-PlaylistTrack -word:$word -playlist:$pl -PropertyName Title
}

foreach($s in "Album", "Artist", "Title") {
	Register-ArgumentCompleter -CommandName Remove-Track -ParameterName $s -ScriptBlock {
		param($_a, $paramName, $word, $_d, $params)

		$params = @{
			Title = $params["Title"]
			Artist = $params["Artist"]
			Album = $params["Album"]
			PropertyName = $paramName
			Playlist = $params["Playlist"]
		}

		Complete-PlaylistTrack -word:$word @params
	}
}

foreach($s in "Album", "Artist", "Title") {
	Register-ArgumentCompleter -CommandName Seek-Queue -ParameterName $s -ScriptBlock {
		param($_a, $paramName, $word, $_d, $params)

		$params = @{
			Title = $params["Title"]
			Artist = $params["Artist"]
			Album = $params["Album"]
			PropertyName = $paramName
		}

		Complete-SeekQueue -word:$word @params
	}
}

Register-ArgumentCompleter -CommandName Enable-MPDOutput, Disable-MPDOutput, Get-MPDOutput, Set-MPDOutputAttribute, Switch-MPDOutput -ParameterName Name -ScriptBlock {
	param($_a, $_b, $word)
	Complete-OutputName -Word:$word
}

Register-ArgumentCompleter -CommandName Enable-MPDOutput, Disable-MPDOutput, Get-MPDOutput, Set-MPDOutputAttribute, Switch-MPDOutput -ParameterName LiteralName -ScriptBlock {
	param($_a, $_b, $word)
	Complete-OutputName -Word:$word -Literal
}
