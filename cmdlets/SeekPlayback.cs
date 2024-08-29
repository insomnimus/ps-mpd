namespace MPD;

[Cmdlet("Seek", "Playback")]
public class SeekPlayback: MpdCmdlet {
	[Parameter(
	Mandatory = true,
	Position = 0,
	ValueFromPipeline = true,
	HelpMessage = "Seek to a relative or absolute position; can be in mm:ss format and prefixed with + or -"
	)]
	public object Value { get; set; }

	protected override void EndProcessing() {
		var secs = 0.0d;
		sbyte relative = 0;

		if (this.Value is TimeSpan ts) {
			secs = Math.Min(ts.TotalSeconds, 0.0);
		} else {
			var s = this.Value?.ToString() ?? "";
			var orig = s;

			if (s.StartsWith('-')) {
				s = s.Substring(1);
				relative = -1;
			} else if (s.StartsWith('+')) {
				s = s.Substring(1);
				relative = 1;
			}

			if (s == "") {
				throw new FormatException($"Invalid duration string: {orig}");
			}

			try {
				secs = parseDuration(s);
			} catch (FormatException e) {
				throw new FormatException($"{e}: {orig}");
			}
		}

		using var mpc = this.Connect();
		mpc.SeekPlayback(relative, secs);
	}

	static double parseDuration(string s) {
		var arr = s.Split(':');
		switch (arr.Length) {
			case 1: {
					// It doesn't have a colon; treat as seconds
					var secs = 0.0d;
					if (double.TryParse(s, out secs)) {
						return secs;
					}
					break;
				}
			case 2: {
					// It's in mm:ss format
					var mins = 0uL;
					var secs = 0.0d;
					if (!ulong.TryParse(arr[0], out mins) || !double.TryParse(arr[1], out secs)) {
						break;
					}
					if (secs < 0.0 || secs >= 60.0) {
						throw new FormatException($"The 'ss' component in a 'mm:ss' format string must be less than 60 and greater than 0");
					}
					return (double)mins * 60.0 + (double)secs;
				}
			case 3: {
					var hrs = 0uL;
					var mins = 0L;
					var secs = 0.0d;
					if (
					!ulong.TryParse(arr[0], out hrs)
					|| !long.TryParse(arr[1], out mins)
					|| !double.TryParse(arr[2], out secs)
					) {
						break;
					}

					if (mins < 0 || mins >= 60) {
						throw new FormatException($"The 'mm' component in a 'hh:mm:ss' format string must be in the range [0..59]");
					} else if (secs < 0 || secs >= 60) {
						throw new FormatException($"The 'ss' component in a 'hh:mm:ss' format string must be in the range [0..59]");
					}
					return (double)hrs * 3600.0 + (double)mins * 60.0 + (double)secs;
				}
			default:
				throw new FormatException($"Too many components in a duration string; most allowed is 'hh:mm:ss'");
		}

		throw new FormatException($"Invalid duration string");
	}
}
