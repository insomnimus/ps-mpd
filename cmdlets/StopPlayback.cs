namespace MPD;

[Cmdlet(VerbsLifecycle.Stop, "Playback")]
public class StopPlayback: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Stop();
	}
}
