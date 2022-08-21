---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Track

## SYNOPSIS
Plays a track.

## SYNTAX

### object (Default)
```
Play-Track [-Track] <Track[]> [-Queue] [<CommonParameters>]
```

### query
```
Play-Track [[-Title] <String[]>] [-Artist <String>] [-Album <String>] [-Queue] [<CommonParameters>]
```

## DESCRIPTION
Plays a track.

## EXAMPLES

### Example 1
```powershell
Play-Track "Never Gonna Give You Up"
```

Plays the track titled "Never Gonna Give You Up".

## PARAMETERS

### -Album
The name of the album

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

### -Queue
Append instead of replacing

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

### -Title
The title of the track

```yaml
Type: String[]
Parameter Sets: query
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Track
Track to play

```yaml
Type: Track[]
Parameter Sets: object
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
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
