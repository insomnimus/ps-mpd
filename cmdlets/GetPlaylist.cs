namespace MPD;

[Cmdlet(VerbsCommon.Get, "Playlist")]
[OutputType(typeof(Playlist))]
public class GetPlaylist: MpdCmdlet {
	[Parameter(Position = 0, ValueFromPipeline = true, HelpMessage = "Name of the playlist")]
	[SupportsWildcards()]
	public string[] Name { get; set; } = ["*"];
	protected override void ProcessRecord() {
		var patterns = this.Name.Select(s => new Pattern(s)).ToArray();
		foreach (var pl in Mpd.FindPlaylists(patterns, this.Cancel)) {
			WriteObject(pl);
		}
	}
}
