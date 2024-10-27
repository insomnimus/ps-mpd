namespace MPD;

[Cmdlet("Play", "Album", DefaultParameterSetName = "query")]
public class PlayAlbum: MpdCmdlet {
	[Parameter(Position = 0, ParameterSetName = "query", HelpMessage = "The album title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "pipe", HelpMessage = "The Album object to play")]
	public Album[] InputObject { get; set; }
	[Parameter(HelpMessage = "Queue the tracks instead")]
	public SwitchParameter Queue { get; set; }
	private TrackFilter filter;
	private List<Album> albums = new();
	private bool isPipe = false;

	protected override void BeginProcessing() {
		this.isPipe = this.ParameterSetName == "object";
		this.filter = new([], this.Artist, this.Title);
	}

	protected override void ProcessRecord() {
		if (isPipe) {
			this.albums.AddRange(this.InputObject);
		} else {
			this.albums.AddRange(Mpd.FindAlbums(this.filter, this.Cancel));
		}
	}

	protected override void EndProcessing() {
		if (this.albums.Count == 0) {
			WriteWarning("No albums found");
			return;
		}
		using var mpc = this.Connect();
		if (!this.Queue) {
			mpc.Clear();
		}
		mpc.Enqueue(this.albums.SelectMany(a => a.Tracks));
		if (!this.Queue) {
			mpc.Play();
		}
		var s = this.Queue ? "Queued" : "Playing";
		if (this.albums.Count == 1) {
			var a = this.albums[0];
			var name = (a.Title, a.Artist) switch {
				("", "") => "1 album",
				("", _) => $"1 Album by {a.Artist}",
				(_, "") => a.Title,
				(_, _) => $"{a.Title} by {a.Artist}",
			};
			WriteInformation($"{s} {name} ({"track".Plural(a.Tracks.Count)})");
		} else {
			WriteInformation($"{s} {this.albums.Count} albums ({"track".Plural(this.albums.Select(a => a.Tracks.Count).Sum())})");
		}
	}
}
