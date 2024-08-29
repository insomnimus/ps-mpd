namespace MPD;

[Cmdlet(VerbsCommon.Get, "MpdVolume")]
[OutputType(typeof(sbyte))]
public class GetMpdVolume: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		WriteObject(mpc.Volume());
	}
}
