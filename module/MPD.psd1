@{
	RootModule = "MPD.psm1"
	ModuleVersion = "2.0.1"
	GUID = "b2e868c9-4a29-4aac-bab8-1edcee8bc142"
	Author = "Taylan Gökkaya"
	Copyright = "Copyright (c) 2024 Taylan Gökkaya <insomnimus@proton.me>"
	Description = "Cmdlets for managing a Music Player Daemon (MPD) instance from Powershell"
	DotNetFrameworkVersion = "8.0"

	FormatsToProcess = @("Track.format.ps1xml", "MpdOutput.format.ps1xml", "MpdStatus.format.ps1xml")
	NestedModules = @("MPD.dll")

	FunctionsToExport = @()
	AliasesToExport = @()
	CmdletsToExport = @(
		"Clear-Queue"
		"Disable-MPDOutput"
		"Enable-MPDOutput"
		"Get-Album"
		"Get-Artist"
		"Get-MPDOutput"
		"Get-MPDStatus"
		"Get-MpdVolume"
		"Get-Playlist"
		"Get-Track"
		"Pause-Playback"
		"Play-Album"
		"Play-Artist"
		"Play-Next"
		"Play-Playlist"
		"Play-Previous"
		"Play-Track"
		"Remove-Track"
		"Save-Playing"
		"Save-Track"
		"Seek-Playback"
		"Seek-Queue"
		"Select-Track"
		"Set-MpdOption"
		"Set-MPDOutputAttribute"
		"Set-MpdVolume"
		"Start-Playback"
		"Stop-Playback"
		"Switch-MPDOutput"
		"Sync-Mpd"
		"Toggle-Playback"
	)

	# FileList = @()

	HelpInfoURI = "https://github.com/insomnimus/ps-mpd"
	PrivateData = @{
		PSData = @{
			LicenseUri = "https://github.com/insomnimus/ps-mpd/blob/main/LICENSE"
			ProjectUri = "https://github.com/insomnimus/ps-mpd"
		}
	}
}
