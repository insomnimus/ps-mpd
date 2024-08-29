---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Set-MpdOption

## SYNOPSIS
Changes MPD options.

## SYNTAX

```
Set-MpdOption [-Consume <TriState>] [-Random <OnOffState>] [-Repeat <OnOffState>] [-Single <TriState>]
[-MixRampDB <Double>] [-ReplayGain <ReplayGain>] [-CrossFade <UInt32>] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

## DESCRIPTION
Changes MPD options.

## EXAMPLES

### Example 1
```powershell
Set-MPDOption -Random on
```


## PARAMETERS

### -Consume
Set consume

```yaml
Type: TriState
Parameter Sets: (All)
Aliases:
Accepted values: Off, On, OneShot

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -CrossFade
Set crossfade in seconds

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -MixRampDB
Set the mixramp threshold in decibels

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Random
Set random

```yaml
Type: OnOffState
Parameter Sets: (All)
Aliases:
Accepted values: Off, On

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Repeat
Set repeat mode

```yaml
Type: OnOffState
Parameter Sets: (All)
Aliases:
Accepted values: Off, On

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ReplayGain
Set replay gain mode

```yaml
Type: ReplayGain
Parameter Sets: (All)
Aliases:
Accepted values: Off, Track, Album, Auto

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Single
Set single state.
When single is activated, playback is stopped after current song, or the song is repeated if the 'repeat' mode is enabled

```yaml
Type: TriState
Parameter Sets: (All)
Aliases:
Accepted values: Off, On, OneShot

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
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

### System.Nullable`1[[MPD.TriState, MPD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]

### System.Nullable`1[[MPD.OnOffState, MPD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]

### System.Nullable`1[[System.Double, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]

### System.Nullable`1[[MPD.ReplayGain, MPD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]

### System.Nullable`1[[System.UInt32, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
