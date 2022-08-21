---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Save-Playing

## SYNOPSIS
Saves the currently playing track to a playlist.

## SYNTAX

### query (Default)
```
Save-Playing [-Name] <String[]> [-AllowDuplicates] [-Force] [<CommonParameters>]
```

### object
```
Save-Playing [-InputObject] <Playlist[]> [-AllowDuplicates] [-Force] [<CommonParameters>]
```

## DESCRIPTION
Saves the currently playing track to a playlist.

## EXAMPLES

### Example 1
```powershell
Save-Playing old-school
```

Saves the currently playing track to the playlist "old-school".

## PARAMETERS

### -AllowDuplicates
Allow adding duplicates

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
The input Playlist object

```yaml
Type: Playlist[]
Parameter Sets: object
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
The name of the playlist to save to

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Playlist[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
