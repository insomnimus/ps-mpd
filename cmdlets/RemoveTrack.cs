namespace MPD;

[Cmdlet(VerbsCommon.Remove, "Track", DefaultParameterSetName = "query")]
public class RemoveTrack: MpdCmdlet {
	[Parameter(Position = 0, Mandatory = true, HelpMessage = "The name of the playlist to remove from")]
	[SupportsWildcards()]
	public string[] Playlist { get; set; }
	[Parameter(
	Mandatory = true,
	ValueFromPipeline = true,
	ValueFromPipelineByPropertyName = true,
	ParameterSetName = "pipe",
	HelpMessage = "The Track object to remove"
	)]
	public Track[] Track { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The track title to match")]
	[SupportsWildcards()]
	public string[] Title { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The artist name to match")]
	[SupportsWildcards()]
	public string[] Artist { get; set; }
	[Parameter(ParameterSetName = "query", HelpMessage = "The album name to match")]
	[SupportsWildcards()]
	public string[] Album { get; set; }
	[Parameter(HelpMessage = "Do not fail if the playlist file was modified outside the MPD module since the time it was loaded")]
	public SwitchParameter Force { get; set; }
	private TrackFilter filter;
	private bool piped;
	private HashSet<string> files = new();
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
			foreach (var t in this.Track) {
				this.files.Add(t.File);
			}
		} else {
			var found = false;
			foreach (var t in Mpd.GetTracks(this.Cancel)) {
				if (this.filter.IsMatch(t)) {
					found = true;
					this.files.Add(t.File);
				}
			}
			if (!found) {
				throw new Exception($"No tracks matched the given criteria");
			}
		}
	}
	protected override void EndProcessing() {
		if (this.files.Count == 0) {
			WriteWarning("No tracks provided");
			return;
		}
		foreach (var pl in this.pls) {
			// Keep the old state in case we have to revert
			var oldTracks = pl._tracks.ToList();
			var n = pl._tracks.RemoveAll(t => {
				if (this.files.Contains(t.File)) {
					WriteVerbose($"Removing {t} from {pl.Name}");
					return true;
				}
				return false;
			});
			if (n <= 0) {
				continue;
			}
			WriteVerbose($"Saving {pl.Name} to {pl.Path}");
			try {
				if (pl.Save(this.Force)) {
					WriteVerbose($"Saved {pl.Name} to {pl.Path}");
				} else {
					pl._tracks = oldTracks;
					WriteError(new ErrorRecord(
				new Exception($"The playlist {pl.Name} has been modified from outside the MPD module, perhaps by another Powershell instance; reverting changes"),
				"MPD.PlaylistModifiedExternally",
				ErrorCategory.LimitsExceeded,
				null
				));
				}
			} catch (Exception) {
				pl._tracks = oldTracks;
				WriteVerbose($"Failed to save {pl.Name}; reverting");
				throw;
			}
			WriteVerbose($"Saved {pl.Name} to {pl.Path}");
		}
	}
}
