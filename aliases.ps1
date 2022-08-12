$aliases = @{
	stra = "Get-Track"
	ptra = "Play-Track"
	sart = "Get-Artist"
	part = "Play-Artist"
	salb = "Get-Album"
	palb = "Play-Album"
	spla = "Get-Playlist"
	pl = "Play-Playlist"
	"<" = "Play-Previous"
	">" = "Play-Next"
	cur = "Get-MPDStatus"
	sct = "Select-Track"
}

foreach($x in $aliases.GetEnumerator()) {
	set-alias $x.name $x.value
}
