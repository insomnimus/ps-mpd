namespace MPD;

[Cmdlet("Pause", "Playback")]
public class PausePlayback: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Pause();
	}
}
