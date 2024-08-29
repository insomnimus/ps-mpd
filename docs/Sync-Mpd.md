---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Sync-MPD

## SYNOPSIS
Synchronizes the current session with the MPD server.

## SYNTAX

```
Sync-MPD [-PlaylistsDir <String>] [-Async] [-MpdHost <String>] [-MpdPort <UInt16>] [-MpdPassword <String>]
[-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Synchronizes the current session with the MPD server.

You need to call this cmdlet for most commands from this module to work correctly; e.g. to get tracks, albums or artists from the music database.
You also need to call this cmdlet with the `-PlaylistsDir` option to let the module know about your local `.m3u` playlists.

Due to the limitations of the MPD protocol and the aims of PS-MPD, the playlists configured via `playlist_directory` in your MPD configuration file are ignored.

## EXAMPLES

### Example 1
```powershell
Sync-MPD -PlaylistsDir D:\Music\Playlists
```


## PARAMETERS

### -Async
Synchronize state in the background.
Note that any error or warning encountered will be lost.

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

### -MpdHost
The MPD server's host string

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MpdPassword
The password to use while connecting to the MPD server

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MpdPort
The port number of the MPD server

```yaml
Type: UInt16
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PlaylistsDir
Path to the directory containing your m3u playlists

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
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

### System.Threading.Tasks.Task

## NOTES

## RELATED LINKS
