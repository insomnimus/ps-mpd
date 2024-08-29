namespace MPD;

[Cmdlet(VerbsCommon.Select, "Track")]
[OutputType(typeof(Track))]
public class SelectTrack: PSCmdlet {
	[Parameter(
		ValueFromPipeline = true,
		ValueFromPipelineByPropertyName = true,
	HelpMessage = "The input object")
	]
	public Track[] InputObject { get; set; }
	[Parameter(Position = 0, HelpMessage = "The track title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	[Parameter(HelpMessage = "The album name to match")]
	[SupportsWildcards()]
	public string[] Album { get; set; }
	private TrackFilter filter;
	protected override void BeginProcessing() {
		this.filter = new(this.Title, this.Artist, this.Album);
	}
	protected override void ProcessRecord() {
		foreach (var t in this.InputObject) {
			if (this.filter.IsMatch(t)) {
				WriteObject(t);
			}
		}
	}
}
