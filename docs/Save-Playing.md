---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Save-Playing

## SYNOPSIS
Adds the currently playing song to a local, synced MPD playlist.

## SYNTAX

```
Save-Playing [-Playlist] <String[]> [-AllowDuplicates] [-Force] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

## DESCRIPTION
Adds the currently playing song to a local, synced MPD playlist.

## EXAMPLES

### Example 1
```powershell
Save-Playing chill
```


## PARAMETERS

### -AllowDuplicates
Add the track even if it's already in the playlist

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
Do not fail if the playlist file was modified outside the MPD module since the time it was loaded

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

### -Playlist
The playlist to add to

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

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
