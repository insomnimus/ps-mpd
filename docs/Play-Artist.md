---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Artist

## SYNOPSIS
Plays all tracks by an artist.

## SYNTAX

### query (Default)
```
Play-Artist [-Name] <String[]> [-Queue] [<CommonParameters>]
```

### object
```
Play-Artist [-InputObject] <Artist[]> [-Queue] [<CommonParameters>]
```

## DESCRIPTION
Plays all tracks by an artist.

## EXAMPLES

### Example 1
```powershell
Play-Artist Metallica
```

Plays all tracks by the artist "Metallica".

## PARAMETERS

### -InputObject
The input Artist object

```yaml
Type: Artist[]
Parameter Sets: object
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Name of the artist

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

### -Queue
Add the artists tracks at the end of the queue

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Artist[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
