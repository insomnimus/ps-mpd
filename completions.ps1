filter :quote {
	if($_ -match "[\s`'`"``\(\)\{\}]") {
		$s = $_.replace("'", "''")
		"'$s'"
	} else {
		$_
	}
}

Register-ArgumentCompleter -CommandName Get-Track -ParameterName title -ScriptBlock {
	param($_a, $_b, $buf, $_d, $params)
	if($buf.startswith("'")) {
		$buf = $buf.trim("'")
	} elseif($buf.startswith('"')) {
		$buf = $buf.trim('"')
	}
	if(!$buf.endswith("*")) {
		$buf += "*"
	}
	$params["title"] = $buf
	Get-Track @params | select-object -ExpandProperty title | script::quote
}
