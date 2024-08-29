namespace MPD;

[Cmdlet("Play", "Previous")]
[OutputType(typeof(Track))]
public class PlayPrevious: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		mpc.Previous();
		var t = mpc.Current();
		if (t is not null) WriteObject(t);
	}
}
