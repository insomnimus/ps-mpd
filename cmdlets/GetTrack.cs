namespace MPD;

[Cmdlet(VerbsCommon.Get, "Track", DefaultParameterSetName = "query")]
[OutputType(typeof(Track))]
public class GetTrack: MpdCmdlet {
	[Parameter(Position = 0, ParameterSetName = "query", HelpMessage = "The track title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; } = [];
	[Parameter(ParameterSetName = "query", HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; } = [];
	[Parameter(ParameterSetName = "query", HelpMessage = "The album name to match")]
	[SupportsWildcards()]
	public string[] Album { get; set; } = [];
	[Parameter(HelpMessage = "Get the playing queue", ParameterSetName = "current-queue")]
	public SwitchParameter Queue { get; set; }
	[Parameter(ParameterSetName = "current-track", HelpMessage = "Get the currently playing song")]
	public SwitchParameter Playing { get; set; }
	private TrackFilter filter;
	protected override void BeginProcessing() {
		this.filter = new(this.Title, this.Artist, this.Album);
	}
	protected override void ProcessRecord() {
		if (this.Playing) {
			using var mpc = this.Connect();
			var t = mpc.Current();
			if (t is not null) {
				WriteObject(t);
			}
			return;
		} else if (this.Queue) {
			using var mpc = this.Connect();
			foreach (var t in mpc.CurrentQueue()) {
				WriteObject(t);
			}
			return;
		}
		foreach (var t in Mpd.GetTracks(this.Cancel)) {
			if (this.filter.IsMatch(t)) {
				WriteObject(t);
			}
		}
	}
}
