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

		Get-Track @params `
		| select-object -ExpandProperty $paramName `
		| where-object { $_ } `
		| sort-object -unique `
		| script::quote
	}
}
