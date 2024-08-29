---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Play-Playlist

## SYNOPSIS
Plays songs from a playlist in your locally synced MPD playlists.

## SYNTAX

### query (Default)
```
Play-Playlist [-Name] <String> [[-Seek] <String>] [-Queue] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

### pipe
```
Play-Playlist -InputObject <Playlist> [[-Seek] <String>] [-Queue] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

## DESCRIPTION
Plays songs from a playlist in your locally synced MPD playlists.

## EXAMPLES

### Example 1
```powershell
Play-Playlist chill
```


## PARAMETERS

### -InputObject
The Playlist object

```yaml
Type: Playlist
Parameter Sets: pipe
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Name of the playlist

```yaml
Type: String
Parameter Sets: query
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
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

### -Seek
Seek to the song in the playlist whose title matches a pattern

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
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

### MPD.Playlist

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
