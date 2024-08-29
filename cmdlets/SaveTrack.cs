namespace MPD;

[Cmdlet(VerbsData.Save, "Track", DefaultParameterSetName = "query")]
public class SaveTrack: MpdCmdlet {
	[Parameter(Position = 0, Mandatory = true, HelpMessage = "The name of the playlist to add to")]
	[SupportsWildcards()]
	public string[] Playlist { get; set; }
	[Parameter(
	Mandatory = true,
	ValueFromPipeline = true,
	ValueFromPipelineByPropertyName = true,
	ParameterSetName = "pipe",
	HelpMessage = "The Track object to add"
	)]
	public Track[] InputObject { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The track title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The album name to match")]
	[SupportsWildcards()]
	public string[] Album { get; set; }
	[Parameter(HelpMessage = "Add the track even if it's already in the playlist")]
	public SwitchParameter AllowDuplicates { get; set; }
	[Parameter(HelpMessage = "Do not fail if the playlist file was modified outside the MPD module since the time it was loaded")]
	public SwitchParameter Force { get; set; }
	private TrackFilter filter;
	private bool piped;
	private List<Track> tracks = new();
	private Playlist[] pls;
	protected override void BeginProcessing() {
		this.piped = this.ParameterSetName == "pipe";
		if (!this.piped && this.Title.Length == 0 && this.Artist.Length == 0 && this.Album.Length == 0) {
			throw new Exception("At least one of -Title, -Artist or -Album must be specified");
		}
		this.pls = Mpd.FindPlaylists(this.Playlist.Select(s => new Pattern(s)).ToArray(), this.Cancel).ToArray();
		if (this.pls.Length == 0) {
			throw new Exception("No playlist found matching the pattern");
		}
		this.filter = new(this.Title, this.Artist, this.Album);
	}
	protected override void ProcessRecord() {
		if (this.piped) {
			this.tracks.AddRange(this.InputObject);
		} else {
			var found = false;
			foreach (var t in Mpd.GetTracks(this.Cancel)) {
				if (this.filter.IsMatch(t)) {
					found = true;
					this.tracks.Add(t);
				}
			}
			if (!found) {
				throw new Exception($"No tracks matched the given criteria");
			}
		}
	}
	protected override void EndProcessing() {
		if (this.tracks.Count == 0) {
			WriteWarning("No tracks provided");
			return;
		}
		var modified = new List<(int oldLength, Playlist pl)>(this.pls.Length);
		foreach (var pl in this.pls) {
			var oldLength = pl.Tracks.Count;
			foreach (var t in this.tracks) {
				if (pl.Add(t, this.AllowDuplicates)) {
					WriteVerbose($"Added {t} to {pl.Name}");
				} else {
					WriteVerbose($"{t} is already in {pl.Name}");
				}
			}
			if (oldLength != pl.Tracks.Count) {
				modified.Add((oldLength, pl));
			}
		}
		foreach (var (oldLength, pl) in modified) {
			WriteVerbose($"Saving playlist {pl.Name} to {pl.Path}");
			try {
				if (pl.Save(this.Force)) {
					WriteVerbose($"Saved playlist {pl.Name} to {pl.Path}");
				} else {
					pl._tracks.Truncate(oldLength);
					WriteError(new ErrorRecord(
					new Exception($"The playlist {pl.Name} has been modified from outside the MPD module, perhaps by another Powershell instance; refusing to overwrite"),
					"MPD.PlaylistModifiedExternally",
					ErrorCategory.LimitsExceeded,
					null
					));
				}
			} catch (Exception e) {
				pl._tracks.Truncate(oldLength);
				WriteError(new ErrorRecord(
				new Exception($"Failed to save playlist {pl.Name}; reverting changes", e),
				"MPD.PlaylistSave",
				ErrorCategory.OperationStopped,
				null
				));
			}
		}
	}
}
