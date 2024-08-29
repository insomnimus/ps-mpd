---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Album

## SYNOPSIS
Plays songs from an album from your synced MPD library.

## SYNTAX

### query (Default)
```
Play-Album [[-Title] <String[]>] [-Artist <String[]>] [-Queue] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

### pipe
```
Play-Album -InputObject <Album[]> [-Queue] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Plays songs from an album from your synced MPD library.

## EXAMPLES

### Example 1
```powershell
Play-Album Clayman
```


## PARAMETERS

### -Artist
The artist name to match

```yaml
Type: String[]
Parameter Sets: query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -InputObject
The Album object to play

```yaml
Type: Album[]
Parameter Sets: pipe
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Queue
Queue the tracks instead

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
The album title to match

```yaml
Type: String[]
Parameter Sets: query
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

### MPD.Album[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
