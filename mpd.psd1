@{
	RootModule = "mpd.psm1"
	ModuleVersion = "0.1.0"
	GUID = 'b2e868c9-4a29-4aac-bab8-1edcee8bc142'
	Author = "Taylan Gökkaya <insomnimus.dev@gmail.com>"
	Copyright = "Copyright (c) 2022 Taylan Gökkaya <insomnimus.dev@gmail.com>"
	Description = "MPD commands for Powershell"
	PowerShellVersion = "6.0"

	FunctionsToExport = "*"
	CmdletsToExport = @()
	VariablesToExport = @()
	AliasesToExport = "*"

	NestedModules = @("completions.ps1")
}
