namespace MPD;

[Cmdlet(VerbsData.Save, "Playing")]
public class SavePlaying: MpdCmdlet {
	[Parameter(Mandatory = true, Position = 0, HelpMessage = "The playlist to add to")]
	[SupportsWildcards()]
	public string[] Playlist { get; set; }
	[Parameter(HelpMessage = "Add the track even if it's already in the playlist")]
	public SwitchParameter AllowDuplicates { get; set; }
	[Parameter(HelpMessage = "Do not fail if the playlist file was modified outside the MPD module since the time it was loaded")]
	public SwitchParameter Force { get; set; }
	protected override void EndProcessing() {
		using var mpc = this.Connect();
		var t = mpc.Current();
		if (t is null) {
			throw new Exception("Nothing is currently playing");
		}
		var pls = Mpd.FindPlaylists(this.Playlist.Select(s => new Pattern(s)).ToArray(), this.Cancel);
		var n = 0;
		foreach (var pl in pls) {
			n++;
			if (!pl.Add(t, this.AllowDuplicates)) {
				WriteInformation($"{t} is already in {pl.Name}");
				continue;
			}
			try {
				if (pl.Save(this.Force)) {
					WriteVerbose($"Saved {pl.Name} to {pl.Path}");
					WriteInformation($"Added {t} to {pl.Name}");
				} else {
					pl._tracks.RemoveAt(pl._tracks.Count - 1);
					WriteError(new ErrorRecord(
				new Exception($"The playlist {pl.Name} has been modified from outside the MPD module, perhaps by another Powershell instance; refusing to overwrite"),
				"MPD.PlaylistModifiedExternally",
				ErrorCategory.LimitsExceeded,
				null
				));
				}
			} catch (Exception e) {
				pl._tracks.RemoveAt(pl._tracks.Count - 1);
				WriteError(new ErrorRecord(
				new Exception($"Failed to save playlist {pl.Name}; reverting changes", e),
				"MPD.PlaylistSave",
				ErrorCategory.OperationStopped,
				null
				));
			}
		}
		if (n == 0) {
			throw new Exception("No playlist matched the given pattern(s)");
		}
	}
}
