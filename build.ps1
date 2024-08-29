#!/usr/bin/env -S pwsh -nologo -noprofile
param([switch] $quiet)

trap {
	write-error -ea continue "error: $_"
	exit 1
}

$verbosity = if($quiet) { "--verbosity=quiet" } else { $null }
dotnet publish --nologo -c release $verbosity $PSScriptRoot
if($LastExitCode -ne 0) {
	throw "failed to build the project"
}

$ErrorActionPreference = "stop"

$p = join-path $PSScriptRoot bin/Release/net8.0/publish
$dest = join-path $PSScriptRoot bin/MPD

if(test-path -lp $dest) {
	remove-item -recurse -lp $dest
}

copy-item -recurse -lp "$PSScriptRoot/Module" $dest
copy-item -lp "$PSScriptRoot/LICENSE", "$PSScriptRoot/readme.md" $dest
get-childitem -lp $p | copy-item -recurse -destination $dest

if(!$quiet) {
	"success: built the module into $dest"
}
