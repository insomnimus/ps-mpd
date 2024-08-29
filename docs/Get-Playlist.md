---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Get-Playlist

## SYNOPSIS
Gets MPD playlists you saved locally.

## SYNTAX

```
Get-Playlist [[-Name] <String[]>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Gets MPD playlists you saved locally from the synced module instance.

## EXAMPLES

### Example 1
```powershell
Get-Playlist chill
```


## PARAMETERS

### -Name
Name of the playlist

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
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

### System.String[]

## OUTPUTS

### MPD.Playlist

## NOTES

## RELATED LINKS
