namespace MPD;

using System.Net;
[Cmdlet(VerbsData.Sync, "MPD")]
[OutputType(typeof(Task))]
public class SyncMpd: MpdCmdlet {
	[Parameter(HelpMessage = "Path to the directory containing your m3u playlists")]
	public string PlaylistsDir { get; set; }
	[Parameter(HelpMessage = "Synchronize state in the background. Note that any error or warning encountered will be lost.")]
	public SwitchParameter Async { get; set; }
	[Parameter(HelpMessage = "The MPD server's host string")]
	public string MpdHost { get; set; } = "127.0.0.1";
	[Parameter(HelpMessage = "The port number of the MPD server")]
	public ushort MpdPort { get; set; } = 6600;
	[Parameter(HelpMessage = "The password to use while connecting to the MPD server")]
	public string MpdPassword { get; set; } = "";
	protected override void ProcessRecord() {
		var m3uFiles = new List<FileInfo>();
		if (!string.IsNullOrEmpty(this.PlaylistsDir)) {
			var dir = new DirectoryInfo(GetUnresolvedProviderPathFromPSPath(this.PlaylistsDir));
			var opts = new EnumerationOptions() {
				IgnoreInaccessible = true,
				RecurseSubdirectories = true,
				ReturnSpecialDirectories = false,
			};
			foreach (var entry in dir.EnumerateFileSystemInfos("*.m3u", opts)) {
				if (entry is FileInfo f) {
					m3uFiles.Add(f);
				}
			}
		}
		var dnsEntry = Dns.GetHostEntry(this.MpdHost);
		if (dnsEntry.AddressList.Length == 0) {
			throw new Exception($"Can't resolve host name {this.MpdHost}");
		}
		var mpdOpts = new MpdOptions(dnsEntry.AddressList[0], this.MpdPort) { Password = this.MpdPassword };
		if (this.Async) {
			WriteObject(Mpd.BackgroundSync(mpdOpts, m3uFiles, this.Cts));
			return;
		}
		var missingFiles = Mpd.Sync(mpdOpts, m3uFiles, this.Cancel);
		foreach (var x in missingFiles) {
			foreach (var song in x.Value) {
				WriteWarning($"The playlist at {x.Key} has a missing song: {song}");
			}
		}
	}
}
