---
external help file: MPD.dll-Help.xml
Module Name: MPD
online version:
schema: 2.0.0
---

# Switch-MPDOutput

## SYNOPSIS
Switches MPD outputs.

## SYNTAX

### name (Default)
```
Switch-MPDOutput [-Name] <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### literal-name
```
Switch-MPDOutput -LiteralName <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### id
```
Switch-MPDOutput -ID <Int32> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### object
```
Switch-MPDOutput -InputObject <MpdOutput> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Switches MPD outputs.

## EXAMPLES

### Example 1
```powershell
Switch-MPDOutput alsa-alt
```


## PARAMETERS

### -ID
The ID of the output

```yaml
Type: Int32
Parameter Sets: id
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject
The MpdOutput object

```yaml
Type: MpdOutput
Parameter Sets: object
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -LiteralName
Exact name of the output (case sensitive, no wildcards)

```yaml
Type: String
Parameter Sets: literal-name
Aliases: ln

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Name of the output

```yaml
Type: String
Parameter Sets: name
Aliases:

Required: True
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

### MPD.MpdOutput

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
