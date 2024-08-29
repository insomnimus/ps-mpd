namespace MPD;

[Cmdlet(VerbsLifecycle.Start, "Playback")]
public class StartPlayback: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Play();
	}
}
