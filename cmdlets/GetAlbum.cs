namespace MPD;

[Cmdlet(VerbsCommon.Get, "Album")]
[OutputType(typeof(Album))]
public class GetAlbum: MpdCmdlet {
	[Parameter(Position = 0, HelpMessage = "The title of the album")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(HelpMessage = "Name of the artist")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	private TrackFilter filter;
	protected override void BeginProcessing() {
		this.filter = new([], this.Artist, this.Title);
	}
	protected override void ProcessRecord() {
		foreach (var a in Mpd.FindAlbums(this.filter, this.Cancel)) {
			WriteObject(a);
		}
	}
}
