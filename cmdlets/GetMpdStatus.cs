namespace MPD;

[Cmdlet(VerbsCommon.Get, "MPDStatus")]
[OutputType(typeof(MpdStatus))]
public class GetMpdStatus: MpdCmdlet {
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		WriteObject(mpc.Status());
	}
}
