---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Get-Playlist

## SYNOPSIS
Gets a playlist.

## SYNTAX

```
Get-Playlist [[-Name] <String[]>] [<CommonParameters>]
```

## DESCRIPTION
Gets a playlist.

For this command to work, the last call to `Sync-MPD` must have set the `-PlaylistsDir` parameter to a directory containing `.m3u` files.

## EXAMPLES

### Example 1
```powershell
Get-Playlist
```

Gets all the playlists.

### Example 2
```powershell
Get-Playlist "*metal*"
```

Gets all the playlists that contain "metal" in their names.

## PARAMETERS

### -Name
Name of the playlist

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: Playlist

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### Playlist

## NOTES

## RELATED LINKS
