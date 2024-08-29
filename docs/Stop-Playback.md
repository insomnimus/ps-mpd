---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Stop-Playback

## SYNOPSIS
Stops playback from the most-recently connected MPD server.

## SYNTAX

```
Stop-Playback [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Stops playback from the most-recently connected MPD server.

Whereas `Pause-Playback` keeps the current position, `Stop-Playback` causes MPD to stop and move to the start of the currently playing song.

## EXAMPLES

### Example 1
```powershell
Stop-Playback
```


## PARAMETERS

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
