---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Get-Track

## SYNOPSIS
Gets tracks from the synced MPD library.

## SYNTAX

### query (Default)
```
Get-Track [[-Title] <String[]>] [-Artist <String[]>] [-Album <String[]>] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

### current-queue
```
Get-Track [-Queue] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### current-track
```
Get-Track [-Playing] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Gets tracks from the synced MPD library.

## EXAMPLES

### Example 1
```powershell
Get-Track "Hallowed Be Thy Name"
```


## PARAMETERS

### -Album
The album name to match

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

### -Playing
Get the currently playing song

```yaml
Type: SwitchParameter
Parameter Sets: current-track
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Queue
Get the playing queue

```yaml
Type: SwitchParameter
Parameter Sets: current-queue
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Title
The track title to match

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

### None

## OUTPUTS

### MPD.Track

## NOTES

## RELATED LINKS
