@{
	RootModule = "mpd.psm1"
	ModuleVersion = "0.1.0"
	GUID = "b2e868c9-4a29-4aac-bab8-1edcee8bc142"
	Author = "Taylan Gökkaya <insomnimus@protonmail.com>"
	Copyright = "Copyright (c) 2022 Taylan Gökkaya <insomnimus@protonmail.com>"
	Description = "MPD commands for Powershell"
	PowerShellVersion = "5.0"

	FunctionsToExport = "[a-z]*"
	CmdletsToExport = @()
	VariablesToExport = @()
	AliasesToExport = "*"

	FormatsToProcess = @("MPDStatus.format.ps1xml")
	NestedModules = @("aliases.ps1", "completions.ps1")
}
