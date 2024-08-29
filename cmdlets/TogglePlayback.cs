namespace MPD;

[Cmdlet("Toggle", "Playback")]
public class TogglePlayback: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Toggle();
	}
}
