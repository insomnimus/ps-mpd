---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Set-MpdVolume

## SYNOPSIS
Adjusts the playback volume of the most-recently connected MPD server.

## SYNTAX

```
Set-MpdVolume [-Amount] <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Adjusts the playback volume of the most-recently connected MPD server.

- If the value starts with a `-`, the volume is decreased by that amount.
- If the value starts with a `+`, the volume is increased by that amount.
- Else, the volume is set to the amount.

## EXAMPLES

### Example 1
```powershell
# Set volume to 20%
Set-MPDVolume 20
# Increase the volume by 10%
Set-MPDVolume +10
# Decrease the volume by 10%
Set-MPDVolume -10
```


## PARAMETERS

### -Amount
Volume amount to adjust: -N, +N or N where N is a number

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
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

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
