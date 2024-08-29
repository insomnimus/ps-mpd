---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Remove-Track

## SYNOPSIS
Removes tracks from playlists in your local, synced MPD playlists.

## SYNTAX

### query (Default)
```
Remove-Track [-Playlist] <String[]> [-Title <String[]>] [-Artist <String[]>] [-Album <String[]>] [-Force]
[-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### pipe
```
Remove-Track [-Playlist] <String[]> -Track <Track[]> [-Force] [-ProgressAction <ActionPreference>]
[<CommonParameters>]
```

## DESCRIPTION
Removes tracks from playlists in your local, synced MPD playlists.

## EXAMPLES

### Example 1
```powershell
Get-Track -Playing | Remove-Track -Playlist chill
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
The name of the playlist to remove from

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

### -Title
The track title to match

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

### -Track
The Track object to remove

```yaml
Type: Track[]
Parameter Sets: pipe
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
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

### MPD.Track[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
