namespace MPD;

[Cmdlet("Play", "Playlist", DefaultParameterSetName = "query")]
public class PlayPlaylist: MpdCmdlet {
	[Parameter(
		Mandatory = true,
		Position = 0,
		ParameterSetName = "query",
		HelpMessage = "Name of the playlist"
	)]
	public string Name { get; set; }
	[Parameter(
		Mandatory = true,
		ValueFromPipeline = true,
		ParameterSetName = "pipe",
		HelpMessage = "The Playlist object"
	)]
	public Playlist InputObject { get; set; }
	[Parameter(Position = 1, HelpMessage = "Seek to the song in the playlist whose title matches a pattern")]
	public string Seek { get; set; }
	[Parameter(HelpMessage = "Queue the tracks instead")]
	public SwitchParameter Queue { get; set; }
	protected override void EndProcessing() {
		var pl = this.InputObject;
		if (this.ParameterSetName == "query") {
			var pat = new Pattern(this.Name);
			try {
				pl = Mpd.GetPlaylists(this.Cancel).First(pl => pat.IsMatch(pl.NormName));
			} catch (InvalidOperationException) {
				throw new Exception($"No playlist found matching the pattern {this.Name}");
			}
		}
		using var mpc = this.Connect();
		var seek = -1;
		if (!string.IsNullOrEmpty(this.Seek)) {
			var pat = new Pattern(this.Seek);
			for (var i = 0; i < pl._tracks.Count; i++) {
				if (pat.IsMatch(pl[i].NormTitle)) {
					seek = i;
					break;
				}
			}
			if (seek < 0) {
				throw new Exception($"No track title in {pl.Name} matched the pattern {this.Seek}");
			}
		}
		if (!this.Queue) {
			mpc.Clear();
			mpc.Enqueue(pl.Tracks);
			if (seek >= 0) {
				mpc.SeekQueue((ulong)seek);
				WriteInformation($"Playing {pl[seek]} from {pl.Name}");
			} else {
				// WriteInformation($"Playing {pl.Name}");
				mpc.Play();
			}
			return;
		}
		if (seek > 0) {
			var currentQueueLength = mpc.QueueLength();
			mpc.Enqueue(pl.Tracks);
			mpc.SeekQueue((ulong)seek + currentQueueLength);
			WriteInformation($"Queued {"track".Plural(pl.Tracks.Count)} from {pl.Name}");
		} else {
			mpc.Enqueue(pl.Tracks);
			WriteInformation($"Queued {pl.Name}");
		}
	}
}
