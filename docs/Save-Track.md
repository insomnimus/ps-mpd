---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Save-Track

## SYNOPSIS
Adds songs to a local, synced MPD playlist.

## SYNTAX

### query (Default)
```
Save-Track [-Playlist] <String[]> [-Title <String[]>] [-Artist <String[]>] [-Album <String[]>]
[-AllowDuplicates] [-Force] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### pipe
```
Save-Track [-Playlist] <String[]> -InputObject <Track[]> [-AllowDuplicates] [-Force]
[-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Adds songs to a local, synced MPD playlist.

## EXAMPLES

### Example 1
```powershell
Get-Track -Artist Metallica | Save-Track metal
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

### -InputObject
The Track object to add

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

### -Playlist
The name of the playlist to add to

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
