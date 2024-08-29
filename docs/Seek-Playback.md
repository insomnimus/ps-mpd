---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Seek-Playback

## SYNOPSIS
Seeks the MPD playback to some position in time.

## SYNTAX

```
Seek-Playback [-Value] <Object> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Seeks the MPD playback to some position in time.

The `-Value` parameter has the form:
- `N` where N is a `double`: treated as seconds
- `mm:ss` where `mm` is an integer and `ss` is a non-negative integer with an optional fractional part; `ss` must be below 60
- `hh:mm:ss` where `hh` is an integer, `mm` is an integer from 0 to 59 and `ss` is a non-negative integer with an optional fractional part; `ss` and `mm` must be below 60

If the string is prefixed with a minus sign (`-`) or a plus sign (`+`), the time to seek is relative to the current position.
Otherwise, the cmdlet will seek to the exact time.

As a special case, you can provide a `System.TimeSpan` object to the `-Value` parameter; it will skip to the exact time (not relative).

## EXAMPLES

### Example 1
```powershell
# Seek forward 10 seconds
Seek-Playback +10
# Seek backward 10 seconds
Seek-Playback -10
# Seek to 01:35 (1 minute 35 seconds)
Seek-Playback 01:35
```


## PARAMETERS

### -Value
Seek to a relative or absolute position; can be in mm:ss format and prefixed with + or -

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
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

### System.Object

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
