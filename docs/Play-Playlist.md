---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Playlist

## SYNOPSIS
Plays a playlist.

## SYNTAX

### query (Default)
```
Play-Playlist [-Name] <String> [[-Track] <String>] [-Queue] [<CommonParameters>]
```

### object
```
Play-Playlist [[-Track] <String>] [-InputObject] <Playlist> [-Queue] [<CommonParameters>]
```

## DESCRIPTION
Plays a playlist.

## EXAMPLES

### Example 1
```powershell
Play-Playlist jazz
```

Plays the playlist "jazz".

### Example 1
```powershell
Get-Playlist jazz | Play-Playlist
```

Plays the playlist "jazz".

## PARAMETERS

### -InputObject
The input playlist object

```yaml
Type: Playlist
Parameter Sets: object
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Name of the playlist

```yaml
Type: String
Parameter Sets: query
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Queue
Add the tracks at the end of the queue

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

### -Track
The track name to start playing from

```yaml
Type: String
Parameter Sets: (All)
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

### Playlist

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
