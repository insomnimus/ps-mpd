namespace MPD;

using System.Management.Automation;

public abstract class MpdCmdlet: PSCmdlet {
	internal CancellationTokenSource Cts = new();
	internal CancellationToken Cancel => this.Cts.Token;

	internal void WriteInformation(string msg) => WriteInformation(new InformationRecord(msg, this.MyInvocation.InvocationName));

	internal MpcContext Connect() {
		var mpc = Mpd.Options.Connect(this.Cts.Token);
		return new MpcContext(mpc, this.Cts.Token);
	}

	protected override void StopProcessing() {
		this.Cts.Cancel();
	}
}
