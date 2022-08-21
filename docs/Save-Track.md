---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Save-Track

## SYNOPSIS
Saves one or more tracks to a playlist.

## SYNTAX

### object (Default)
```
Save-Track [-Playlist] <Object[]> [-InputObject] <Track[]> [-NoSave] [-AllowDuplicates] [-Force]
 [<CommonParameters>]
```

### query
```
Save-Track [-Playlist] <Object[]> [[-title] <String[]>] [-Artist <String>] [-Album <String>] [-NoSave]
 [-AllowDuplicates] [-Force] [<CommonParameters>]
```

## DESCRIPTION
Saves one or more tracks to a playlist.

## EXAMPLES

### Example 1
```powershell
Get-Playlist metal, rock | Select-Object -ExpandProperty Tracks | Save-Track metal-and-rock
```

Combines the playlists "rock" and "metal" into the playlist "metal-and-rock".

## PARAMETERS

### -Album
Name of the album

```yaml
Type: String
Parameter Sets: query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AllowDuplicates
Allow adding tracks even if they are alreawdy in the playlist

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Artist
The name of the artist

```yaml
Type: String
Parameter Sets: query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Overwrite any externally made changes since last sync

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject
The input Track object

```yaml
Type: Track[]
Parameter Sets: object
Aliases: Track

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -NoSave
Do not save the playlist to disk

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: NS

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Playlist
The playlist object or name of the playlist to save the track to

```yaml
Type: Object[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -title
Name of the track to save

```yaml
Type: String[]
Parameter Sets: query
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Track[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
