---
external help file: MPD-help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Sync-Mpd

## SYNOPSIS
Syncs the MPD database with the current Powershell session.

## SYNTAX

```
Sync-Mpd [[-PlaylistsDir] <String>] [-UpdateMPD] [<CommonParameters>]
```

## DESCRIPTION
Syncs the MPD database with the current Powershell session.

The MPD music library is kept in-memory (just the metadata) for fast tab completions and operations.
Use this command to synchronize the current state of MPD with your Powershell session.

This is not permanent; you need to call this command each time you open up a new session.
You also need to call this command if the MPD database changes.

For playlists to be recognized, you need to specify the `-PlaylistsDir` parameter.
Files with the `m3u` extension under this directory will be loaded as playlists.

## EXAMPLES

### Example 1
```powershell
Sync-MPD -PlaylistsDir D:\music\playlists
```

Synchronizes the session with the MPD server.

## PARAMETERS

### -PlaylistsDir
Absolute path of the playlists directory

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UpdateMPD
Update mpd before loading songs

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
