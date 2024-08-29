namespace MPD;

[Cmdlet("Play", "Artist", DefaultParameterSetName = "query")]
public class PlayArtist: MpdCmdlet {
	[Parameter(Mandatory = true, Position = 0, ParameterSetName = "query", HelpMessage = "The album title to match")]
	[SupportsWildcards()]
	public string[] Name { get; set; }
	[Parameter(ValueFromPipeline = true, ParameterSetName = "object", HelpMessage = "The Artist object")]
	public Artist[] InputObject { get; set; }
	[Parameter(HelpMessage = "Queue the tracks instead")]
	public SwitchParameter Queue { get; set; }
	private List<Artist> artists = new();
	private bool isPipe = false;
	protected override void BeginProcessing() {
		this.isPipe = this.ParameterSetName == "object";
	}
	protected override void ProcessRecord() {
		if (isPipe) {
			this.artists.AddRange(this.InputObject);
		} else {
			this.artists.AddRange(Mpd.FindArtists(this.Name.Select(s => new Pattern(s)).ToArray(), this.Cancel));
		}
	}
	protected override void EndProcessing() {
		if (this.artists.Count == 0) {
			WriteWarning("No artists found");
			return;
		}
		using var mpc = this.Connect();
		if (!this.Queue) {
			mpc.Clear();
		}
		mpc.Enqueue(this.artists.SelectMany(a => a.Albums.SelectMany(a => a.Tracks)));
		if (!this.Queue) {
			mpc.Play();
		}
		var s = this.Queue ? "Queued" : "Playing";
		var totalAlbums = this.artists.Select(a => a.Albums.Count).Sum();
		var totalTracks = this.artists.Select(a => a.Albums.Select(a => a.Tracks.Count).Sum()).Sum();
		if (this.artists.Count == 1) {
			var a = this.artists[0];
			var name = (a.Name == "") ? "1 artist" : a.Name;
			WriteInformation($"{s} {name} ({"album".Plural(totalAlbums)}, {"track".Plural(totalTracks)})");
		} else {
			WriteInformation($"{s} {"artist".Plural(this.artists.Count)} ({"album".Plural(totalAlbums)}, {"track".Plural(totalTracks)})");
		}
	}
}
