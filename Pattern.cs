namespace MPD;

using System.Management.Automation;

internal struct Pattern {
	private string normalizedText;
	private WildcardPattern glob;
	private bool hasWildcard;

	internal Pattern(string pattern) {
		pattern = pattern.Normal();

		this.hasWildcard = WildcardPattern.ContainsWildcardCharacters(pattern) || pattern.Contains('`');

		this.normalizedText = pattern;
		if (this.hasWildcard) {
			this.glob = new(pattern, WildcardOptions.Compiled | WildcardOptions.CultureInvariant);
		} else {
			this.glob = new("", WildcardOptions.None);
		}
	}

	internal bool IsMatch(string normalized) {
		return (this.hasWildcard && this.glob.IsMatch(normalized)) || normalized == this.normalizedText;
	}
}

internal class TrackFilter {
	private Pattern[] title;
	private Pattern[] artist;
	private Pattern[] album;

	internal TrackFilter(string[] title, string[] artist, string[] album) {
		this.title = title?.Select(s => new Pattern(s)).ToArray() ?? [];
		this.artist = artist?.Select(s => new Pattern(s)).ToArray() ?? [];
		this.album = album?.Select(s => new Pattern(s)).ToArray() ?? [];
	}

	internal bool IsMatch(Track t) {
		var nope = (this.artist.Length > 0 && (t.NormArtist == "" || !this.artist.Any(g => g.IsMatch(t.NormArtist))))
		|| (this.album.Length > 0 && (t.NormAlbum == "" || !this.album.Any(g => g.IsMatch(t.NormAlbum))))
		|| (this.title.Length > 0 && (t.NormTitle == "" || !this.title.Any(g => g.IsMatch(t.NormTitle))));

		return !nope;
	}
}
