namespace MPD;

[Cmdlet(VerbsCommon.Clear, "Queue")]
public class ClearQueue: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Clear();
	}
}
