---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Seek-Queue

## SYNOPSIS
Seeks to a track in the current queue.

## SYNTAX

```
Seek-Queue [[-Title] <String>] [-Artist <String>] [-Album <String>] [<CommonParameters>]
```

## DESCRIPTION
Seeks to a track in the current queue.

## EXAMPLES

### Example 1
```powershell
Seek-Queue -Artist Eminem
```

Seeks to the first song by the artist "Eminem" in the currently playing queue.

## PARAMETERS

### -Album
Name of the album

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Artist
The name of the artist

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Title
The title of a song from the current queue to seek to

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
