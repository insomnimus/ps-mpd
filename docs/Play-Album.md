---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Album

## SYNOPSIS
Plays an album.

## SYNTAX

### query (Default)
```
Play-Album [-Artist <String>] [[-Name] <String[]>] [-Queue] [<CommonParameters>]
```

### object
```
Play-Album [-InputObject] <Album[]> [-Queue] [<CommonParameters>]
```

## DESCRIPTION
Plays an album.

You can pipe Album objects into this command.

## EXAMPLES

### Example 1
```powershell
Get-Album "Lawless Darkness" | Play-Album
```

Searches for the album "Lawless Darkness" and plays it.

### Example 2
```powershell
Play-Album -Artist Emperor
```

Plays all albums by the artist "Emperor".


## PARAMETERS

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

### -InputObject
The Album object

```yaml
Type: Album[]
Parameter Sets: object
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Name of the album

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

### -Queue
Add the album at the end of the queue

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

### Album[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
