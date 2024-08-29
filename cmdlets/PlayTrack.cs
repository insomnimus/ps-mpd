namespace MPD;

[Cmdlet("Play", "Track", DefaultParameterSetName = "query")]
public class PlayTrack: MpdCmdlet {
	[Parameter(Position = 0, ParameterSetName = "query", HelpMessage = "The track title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The album name to match")]
	[SupportsWildcards()]
	public string[] Album { get; set; }
	[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "pipe", HelpMessage = "The Track object to play")]
	public Track[] InputObject { get; set; }
	[Parameter(HelpMessage = "Queue the tracks instead")]
	public SwitchParameter Queue { get; set; }
	private TrackFilter filter;
	private List<Track> tracks = new();
	private bool isPipe = false;
	protected override void BeginProcessing() {
		this.isPipe = this.ParameterSetName == "object";
		this.filter = new(this.Title, this.Artist, this.Album);
	}
	protected override void ProcessRecord() {
		if (isPipe) {
			this.tracks.AddRange(this.InputObject);
		} else {
			foreach (var t in Mpd.GetTracks(this.Cancel)) {
				if (this.filter.IsMatch(t)) {
					this.tracks.Add(t);
				}
			}
		}
	}
	protected override void EndProcessing() {
		if (this.tracks.Count == 0) {
			WriteWarning("No tracks found");
			return;
		}
		using var mpc = this.Connect();
		if (!this.Queue) {
			mpc.Clear();
		}
		mpc.Enqueue(this.tracks);
		var s = this.Queue ? "Queued" : "Playing";
		if (this.tracks.Count == 1) {
			WriteInformation($"{s} {this.tracks[0]}");
		} else {
			WriteInformation($"{s} {this.tracks.Count} tracks");
		}
		if (!this.Queue) {
			mpc.Play();
		}
	}
}
