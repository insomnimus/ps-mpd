---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Remove-Track

## SYNOPSIS
Removes tracks from a playlist.

## SYNTAX

### object (Default)
```
Remove-Track [-Playlist] <Object[]> [-InputObject] <Track[]> [-NoSave] [-Force] [<CommonParameters>]
```

### query
```
Remove-Track [-Playlist] <Object[]> [[-title] <String[]>] [-Artist <String>] [-Album <String>] [-NoSave]
 [-Force] [<CommonParameters>]
```

## DESCRIPTION
Removes tracks from a playlist.

## EXAMPLES

### Example 1
```powershell
Get-MPDStatus | Remove-track chill
```

Removes the currently playing track from the playlist "chill".

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
Force overwriting any externally made changes since last sync

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
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Playlist
The playlist object or name of the playlist

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
Name of the track to remove

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
