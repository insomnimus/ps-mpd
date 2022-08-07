filter :quote {
	if($_ -match "[\s`'`"``\(\)\{\}]") {
		$s = $_.replace("'", "''")
		"'$s'"
	} else {
		$_
	}
}

"Artist", "Album", "Title" | foreach-object {
	Register-ArgumentCompleter -CommandName Get-Track, Play-Track -ParameterName $_ -ScriptBlock {
		param($_a, $paramName, $buf, $_d, $params)
		if($buf.startswith("'")) {
			$buf = $buf.trim("'")
		} elseif($buf.startswith('"')) {
			$buf = $buf.trim('"')
		}
		if(!$buf.endswith("*")) {
			$buf += "*"
		}

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
		if($buf.startswith("'")) {
			$buf = $buf.trim("'")
		} elseif($buf.startswith('"')) {
			$buf = $buf.trim('"')
		}
		if(!$buf.endswith("*")) {
			$buf += "*"
		}

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

	if($buf.startswith("'")) {
		$buf = $buf.trim("'")
	} elseif($buf.startswith('"')) {
		$buf = $buf.trim('"')
	}

	if(!$buf.endswith("*")) {
		$buf += "*"
	}

	$script:MPD.artists.Keys `
	| where-object { $_ -like $buf } `
	| script::quote
}
