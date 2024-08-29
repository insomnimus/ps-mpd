namespace MPD;

[Cmdlet("Seek", "Queue")]
public class SeekQueue: MpdCmdlet {
	[Parameter(Position = 0, HelpMessage = "The track title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	[Parameter(HelpMessage = "The album name to match")]
	[SupportsWildcards()]
	public string[] Album { get; set; }
	protected override void ProcessRecord() {
		if (this.Title is [] && this.Artist is [] && this.Album is []) {
			return;
		}
		var filter = new TrackFilter(this.Title, this.Artist, this.Album);
		var i = 0uL;
		using var mpc = this.Connect();
		foreach (var t in mpc.CurrentQueue()) {
			if (filter.IsMatch(t)) {
				mpc.SeekQueue(i);
				WriteInformation($"Playing track #{i + 1} ({t})");
				return;
			}
			i++;
		}
		throw new Exception("No track in the current queue matched the criteria");
	}
}
