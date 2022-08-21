---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Select-Track

## SYNOPSIS
Filters tracks by title, artist and album.

## SYNTAX

```
Select-Track [[-Title] <String[]>] [-Artist <String[]>] [-Album <String[]>] [-First <UInt32>]
 [-InputObject <Track[]>] [<CommonParameters>]
```

## DESCRIPTION
Filters tracks by title, artist and album.

Every parameter specified will narrow down the filter.
Specifying multiple values per-parameter will make the filter more general.

## EXAMPLES

### Example 1
```powershell
Get-Track | Select-Track -Artist Watain
```

Gets all the tracks and selects those by the artist "Watain".

## PARAMETERS

### -Album
The album name to match

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Artist
The Artist name to match

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -First
Select first N tracks that match the criteria

```yaml
Type: UInt32
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
Parameter Sets: (All)
Aliases: Track

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Title
The track title to match

```yaml
Type: String[]
Parameter Sets: (All)
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

### Track[]

## OUTPUTS

### Track

## NOTES

## RELATED LINKS
