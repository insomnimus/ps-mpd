---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Get-Album

## SYNOPSIS
Gets an album.

## SYNTAX

```
Get-Album [[-Name] <String[]>] [-Artist <String>] [<CommonParameters>]
```

## DESCRIPTION
Gets an album.

## EXAMPLES

### Example 1
```powershell
Get-Album
```

Gets all the albums.

### Example 2
```powershell
Get-Album -artist Insomnium
```

Gets all the albums by the artist "Insomnium".

## PARAMETERS

### -Artist
Name of the artist

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Name of the album

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

### None

## OUTPUTS

### Album

## NOTES

## RELATED LINKS
