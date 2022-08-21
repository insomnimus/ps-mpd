---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Get-Track

## SYNOPSIS
Gets a track.

## SYNTAX

### query (Default)
```
Get-Track [[-Title] <String[]>] [-Artist <String>] [-Album <String>] [<CommonParameters>]
```

### current-track
```
Get-Track [-Playing] [<CommonParameters>]
```

### current-queue
```
Get-Track [-Queue] [<CommonParameters>]
```

## DESCRIPTION
Gets a track.

## EXAMPLES

### Example 1
```powershell
Get-Track -artist Insomnium "while *"
```

Gets all the tracks by the artist "Insomnium" that start with "while".

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
Name of the artist

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

### -Playing
Get the currently playing song

```yaml
Type: SwitchParameter
Parameter Sets: current-track
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Queue
Get the playing queue

```yaml
Type: SwitchParameter
Parameter Sets: current-queue
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Title
Title of the track

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### Track

## NOTES

## RELATED LINKS
