namespace MPD;

[Cmdlet(VerbsCommon.Get, "Artist")]
[OutputType(typeof(Artist))]
public class GetArtist: MpdCmdlet {
	[Parameter(Position = 0, ValueFromPipeline = true, HelpMessage = "Name of the artist")]
	[SupportsWildcards()]
	public string[] Name { get; set; } = ["*"];
	protected override void ProcessRecord() {
		foreach (var a in Mpd.FindArtists(this.Name.Select(s => new Pattern(s)).ToArray(), this.Cancel)) {
			WriteObject(a);
		}
	}
}
