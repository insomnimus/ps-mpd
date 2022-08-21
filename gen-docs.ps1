pushd $PSScriptRoot

import-module -force "$PSScriptRoot/MPD" -ea stop -disableNameChecking

$m = get-module MPD -ea stop
# $commands = $m.ExportedCommands.Values | where-object { $_.CommandType -eq "function" }
$commands = get-command -module MPD -ea stop ` | sort-object -property Noun

$helps = $commands | foreach-object {
	$aliases = get-alias -definition $_.name -ea ignore
	if($aliases) {
		$aliases = $aliases.name | foreach-object { "- ``$_``" } | join-string -separator "`n" -outputPrefix "Aliases:`n"
	}
		@"
### [$($_.name)](docs/$($_.name).md)
$aliases
$(get-help $_.name | join-string -separator "`n`n" { $_.description.text })
"@
} | join-string -separator "`n`n" { $_.trim() }

copy-item -force documentation.md-tpl documentation.md
$helps >> documentation.md

popd