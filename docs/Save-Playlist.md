---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Save-Playlist

## SYNOPSIS
Saves a playlist to disk.

## SYNTAX

### query (Default)
```
Save-Playlist [-Name] <String[]> [<CommonParameters>]
```

### object
```
Save-Playlist [-InputObject] <Playlist[]> [<CommonParameters>]
```

## DESCRIPTION
Saves a playlist to disk.

By default commands that modify playlists will save the changes back to the disk.
This command is included for completeness.

## EXAMPLES

### Example 1
```powershell
Get-Playlist | Save-Playlist
```

Saves all the playlists to disk.

## PARAMETERS

### -InputObject
The Playlist object

```yaml
Type: Playlist[]
Parameter Sets: object
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Name of the playlist to save

```yaml
Type: String[]
Parameter Sets: query
Aliases:

Required: True
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

### System.Object
## NOTES

## RELATED LINKS
