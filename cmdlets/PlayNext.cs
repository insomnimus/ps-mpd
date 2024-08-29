namespace MPD;

[Cmdlet("Play", "Next")]
[OutputType(typeof(Track))]
public class PlayNext: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Next();
		var t = mpc.Current();
		if (t is not null) WriteObject(t);
	}
}
