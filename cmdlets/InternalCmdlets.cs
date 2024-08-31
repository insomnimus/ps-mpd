namespace MPD;

using System.Text;
internal static class Completions {
	internal static string Quote(string s, StringBuilder buf) {
		// Characters that need to be quoted
		const string SPECIALS = " '`&\",|()[]{}<>$@;\t\n\r";
		var needQuoting = s.Any(c => SPECIALS.Contains(c));
		if (!needQuoting) {
			return s;
		}
		buf.Clear();
		buf.Append('\'');
		foreach (var c in s) {
			if (c == '\'') {
				buf.Append("''");
			} else {
				buf.Append(c);
			}
		}
		buf.Append('\'');
		return buf.ToString();
	}
	internal static string NormalizeInput(string s) {
		if (string.IsNullOrEmpty(s) || s == "*") {
			return "*";
		}
		if (s.Length == 1) {
			return s + "*";
		}
		// Trim surrounding (matching) quotation
		var c = s[0];
		if ((c == '\'' || c == '"') && s[^1] == c) {
			if (s.Length == 2) {
				return "*";
			} else if (s[^-2] == '*') {
				return s[1..^1];
			} else {
				return s[1..^1] + "*";
			}
		}
		if (s.EndsWith('*')) {
			return s;
		} else {
			return s + "*";
		}
	}
}
public abstract class MpdCompletionCmdlet: Cmdlet {
	internal virtual bool sort => true;
	internal CancellationTokenSource Cts = new();
	internal CancellationToken Cancel => this.Cts.Token;
	[Parameter()]
	[AllowEmptyString()]
	[AllowNull()]
	public string Word { get; set; }
	internal List<(string norm, string name)> matches = new();
	internal HashSet<string> seen = new();
	internal Pattern pattern;
	protected override void BeginProcessing() {
		this.pattern = new(Completions.NormalizeInput(this.Word));
	}
	protected override void EndProcessing() {
		if (this.matches.Count == 0) {
			return;
		}
		var buf = new StringBuilder(256);
		if (this.sort) {
			this.matches.Sort((a, b) => a.norm.CompareTo(b.norm));
		}
		foreach (var x in this.matches) {
			WriteObject(Completions.Quote(x.name, buf));
		}
	}
}
// Completion for Get-Artist and Play-Artist
[Cmdlet("Complete", "Artist")]
public class CompleteArtist: MpdCompletionCmdlet {
	protected override void ProcessRecord() {
		foreach (var t in Mpd.GetTracks(this.Cancel)) {
			if (t.Artist != "" && this.pattern.IsMatch(t.NormArtist) && this.seen.Add(t.Artist.ToUpperInvariant())) {
				this.matches.Add((t.NormArtist, t.Artist));
			}
		}
	}
}
// Completion for Get-Album and Play-Album
[Cmdlet("Complete", "Album")]
public class CompleteAlbum: MpdCompletionCmdlet {
	[Parameter(Mandatory = true)]
	[ValidateSet("Title", "Artist")]
	public string PropertyName { get; set; }
	[Parameter()]
	public string[] Artist { get; set; }
	[Parameter()]
	public string[] Title { get; set; }
	protected override void ProcessRecord() {
		var isArtist = this.PropertyName == "Artist";
		if (isArtist) this.Artist = [];
		else this.Title = [];
		var filter = new TrackFilter([], this.Artist, this.Title);
		foreach (var t in Mpd.GetTracks(this.Cancel)) {
			if (isArtist && t.Artist != "" && this.pattern.IsMatch(t.NormArtist) && filter.IsMatch(t) && this.seen.Add(t.Artist.ToUpperInvariant())) {
				this.matches.Add((t.NormArtist, t.Artist));
			} else if (!isArtist && t.Album != "" && this.pattern.IsMatch(t.NormAlbum) && filter.IsMatch(t) && this.seen.Add(t.Album.ToUpperInvariant())) {
				this.matches.Add((t.NormAlbum, t.Album));
			}
		}
	}
}
// Completion for Get-Track, Save-Track and Play-Track
[Cmdlet("Complete", "Track")]
public class CompleteTrack: MpdCompletionCmdlet {
	[Parameter(Mandatory = true)]
	[ValidateSet("Title", "Artist", "Album")]
	public string PropertyName { get; set; }
	[Parameter()]
	public string[] Title { get; set; }
	[Parameter()]
	public string[] Artist { get; set; }
	[Parameter()]
	public string[] Album { get; set; }
	protected override void ProcessRecord() {
		var filter = new TrackFilter(this.Title, this.Artist, this.Album);
		var (isTitle, isArtist, isAlbum) = (false, false, false);
		if (this.PropertyName == "Title") {
			this.Title = [];
			isTitle = true;
		} else if (this.PropertyName == "Artist") {
			this.Artist = [];
			isArtist = true;
		} else {
			this.Album = [];
			isAlbum = true;
		}
		foreach (var t in Mpd.GetTracks(this.Cancel)) {
			if (isTitle && t.Title != "" && this.pattern.IsMatch(t.NormTitle) && filter.IsMatch(t) && this.seen.Add(t.Title.ToUpperInvariant())) {
				this.matches.Add((t.NormTitle, t.Title));
			} else if (isArtist && t.Artist != "" && this.pattern.IsMatch(t.NormArtist) && filter.IsMatch(t) && this.seen.Add(t.Artist.ToUpperInvariant())) {
				this.matches.Add((t.NormArtist, t.Artist));
			} else if (isAlbum && t.Album != "" && this.pattern.IsMatch(t.NormAlbum) && filter.IsMatch(t) && this.seen.Add(t.Album.ToUpperInvariant())) {
				this.matches.Add((t.NormAlbum, t.Album));
			}
		}
	}
}
// Completion for Get-Playlist, Save-Track, Remove-Track
[Cmdlet("Complete", "Playlist")]
public class CompletePlaylist: MpdCompletionCmdlet {
	protected override void ProcessRecord() {
		foreach (var p in Mpd.GetPlaylists(this.Cancel)) {
			if (this.pattern.IsMatch(p.NormName) && this.seen.Add(p.Name.ToUpperInvariant())) {
				this.matches.Add((p.NormName, p.Name));
			}
		}
	}
}
[Cmdlet("Complete", "PlaylistTrack")]
public class CompletePlaylistTrack: MpdCompletionCmdlet {
	internal override bool sort => this.PropertyName != "Title";
	[Parameter()]
	public string[] Playlist { get; set; }
	[Parameter(Mandatory = true)]
	[ValidateSet("Title", "Artist", "Album")]
	public string PropertyName { get; set; }
	[Parameter()]
	[AllowEmptyCollection()]
	[AllowNull()]
	[AllowEmptyString()]
	public string[] Title { get; set; }
	[Parameter()]
	[AllowEmptyCollection()]
	[AllowNull()]
	[AllowEmptyString()]
	public string[] Artist { get; set; }
	[Parameter()]
	[AllowEmptyCollection()]
	[AllowNull()]
	[AllowEmptyString()]
	public string[] Album { get; set; }
	protected override void ProcessRecord() {
		if (this.Playlist is null || this.Playlist is []) {
			return;
		}
		var (isTitle, isArtist, isAlbum) = (false, false, false);
		if (this.PropertyName == "Title") {
			this.Title = [];
			isTitle = true;
		} else if (this.PropertyName == "Artist") {
			this.Artist = [];
			isArtist = true;
		} else {
			this.Album = [];
			isAlbum = true;
		}
		var filter = new TrackFilter(this.Title, this.Artist, this.Album);
		var playlistPatterns = this.Playlist.Select(s => new Pattern(s)).ToArray();
		var pls = Mpd.FindPlaylists(playlistPatterns, this.Cancel);
		foreach (var pl in pls) {
			foreach (var t in pl.Tracks) {
				if (isTitle && t.Title != "" && this.pattern.IsMatch(t.NormTitle) && filter.IsMatch(t) && this.seen.Add(t.Title.ToUpperInvariant())) {
					this.matches.Add((t.NormTitle, t.Title));
				} else if (isArtist && t.Artist != "" && this.pattern.IsMatch(t.NormArtist) && filter.IsMatch(t) && this.seen.Add(t.Artist.ToUpperInvariant())) {
					this.matches.Add((t.NormArtist, t.Artist));
				} else if (isAlbum && t.Album != "" && this.pattern.IsMatch(t.NormAlbum) && filter.IsMatch(t) && this.seen.Add(t.Album.ToUpperInvariant())) {
					this.matches.Add((t.NormAlbum, t.Album));
				}
			}
		}
	}
}
[Cmdlet("Complete", "SeekQueue")]
public class CompleteSeekQueue: MpdCompletionCmdlet {
	internal override bool sort => false;
	[Parameter(Mandatory = true)]
	[ValidateSet("Title", "Artist", "Album")]
	public string PropertyName { get; set; }
	[Parameter()]
	[AllowEmptyCollection()]
	[AllowNull()]
	public string Title { get; set; }
	[Parameter()]
	[AllowEmptyCollection()]
	[AllowNull()]
	public string Artist { get; set; }
	[Parameter()]
	[AllowEmptyCollection()]
	[AllowNull()]
	public string Album { get; set; }

	protected override void ProcessRecord() {
		var (isTitle, isArtist, isAlbum) = (false, false, false);
		if (this.PropertyName == "Title") {
			this.Title = "";
			isTitle = true;
		} else if (this.PropertyName == "Artist") {
			this.Artist = "";
			isArtist = true;
		} else {
			this.Album = "";
			isAlbum = true;
		}

		var filter = new TrackFilter(
			string.IsNullOrEmpty(this.Title) ? [] : [this.Title],
			string.IsNullOrEmpty(this.Artist) ? [] : [this.Artist],
			string.IsNullOrEmpty(this.Album) ? [] : [this.Album]
		);

		using var mpc = new MpcContext(Mpd.Options.Connect(this.Cancel), this.Cancel);
		var tracks = mpc.CurrentQueue();
		foreach (var t in tracks) {
			if (isTitle && t.Title != "" && this.pattern.IsMatch(t.NormTitle) && filter.IsMatch(t) && this.seen.Add(t.Title.ToUpperInvariant())) {
				this.matches.Add((t.NormTitle, t.Title));
			} else if (isArtist && t.Artist != "" && this.pattern.IsMatch(t.NormArtist) && filter.IsMatch(t) && this.seen.Add(t.Artist.ToUpperInvariant())) {
				this.matches.Add((t.NormArtist, t.Artist));
			} else if (isAlbum && t.Album != "" && this.pattern.IsMatch(t.NormAlbum) && filter.IsMatch(t) && this.seen.Add(t.Album.ToUpperInvariant())) {
				this.matches.Add((t.NormAlbum, t.Album));
			}
		}
	}
	protected override void StopProcessing() {
		this.Cts.Cancel();
	}
}
[Cmdlet("Complete", "OutputName")]
public class CompleteOutputName: MpdCompletionCmdlet {
	[Parameter()]
	public SwitchParameter Literal { get; set; }
	protected override void ProcessRecord() {
		List<MpdOutput> outputs;
		try {
			using var mpc = new MpcContext(Mpd.Options.Connect(this.Cancel), this.Cancel);
			outputs = mpc.Outputs();
		} catch (Exception) {
			return;
		}
		var s = Completions.NormalizeInput(this.Word);
		WildcardPattern glob = null;
		if (this.Literal) {
			s = s.Substring(0, s.Length - 1).ToUpperInvariant();
		} else {
			glob = new(s, WildcardOptions.Compiled | WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);
		}
		foreach (var o in outputs) {
			var nameUpper = o.Name.ToUpperInvariant();
			if ((glob is not null && glob.IsMatch(o.Name)) || (glob is null && nameUpper.StartsWith(s))) {
				this.matches.Add((nameUpper, o.Name));
			}
		}
	}
}
