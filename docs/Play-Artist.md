---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Artist

## SYNOPSIS
Plays songs from an artist from your synced MPD library.

## SYNTAX

```
Play-Artist [-Name] <String[]> -InputObject <Artist[]> [-Queue] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

## DESCRIPTION
Plays songs from an artist from your synced MPD library.

## EXAMPLES

### Example 1
```powershell
Play-Artist Metallica
```


## PARAMETERS

### -InputObject
The Album object to play

```yaml
Type: Artist[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
The album title to match

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
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

### MPD.Artist[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
