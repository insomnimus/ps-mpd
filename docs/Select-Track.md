---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Select-Track

## SYNOPSIS
Filters Tracks coming from the pipeline based on the title, album and artist.

## SYNTAX

```
Select-Track [-InputObject <Track[]>] [[-Title] <String[]>] [-Artist <String[]>] [-Album <String[]>]
[-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Filters Tracks coming from the pipeline based on the title, album and artist.

## EXAMPLES

### Example 1
```powershell
Get-Track | Select-Track -Title "The *"
```


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
Accept wildcard characters: True
```

### -Artist
The artist name to match

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -InputObject
The input object

```yaml
Type: Track[]
Parameter Sets: (All)
Aliases:

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
Accept wildcard characters: True
```

### -ProgressAction
N/A

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### MPD.Track[]

## OUTPUTS

### MPD.Track

## NOTES

## RELATED LINKS
