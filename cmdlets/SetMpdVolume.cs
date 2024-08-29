namespace MPD;

[Cmdlet(VerbsCommon.Set, "MpdVolume")]
public class SetMpdVolume: MpdCmdlet {
	[Parameter(
		Mandatory = true,
		Position = 0,
		HelpMessage = "Volume amount to adjust: -N, +N or N where N is a number"
	)]
	public string Amount { get; set; }
	protected override void ProcessRecord() {
		var s = this.Amount;
		var n = 0L;
		if (!long.TryParse(s, out n)) {
			throw new ArgumentException($"Can't parse amount: {s}", "Amount");
		}
		if (n > 100) {
			throw new ArgumentOutOfRangeException($"Value can't be more than 100: {n}", "Amount");
		} else if (n < -100) {
			throw new ArgumentOutOfRangeException($"Value can't be less than 100: {n}", "Amount");
		}
		using var mpc = this.Connect();
		if (s[0] != '-' && s[0] != '+') {
			mpc.SetVolume((byte)n);
			WriteInformation($"volume: {n}%");
		} else {
			mpc.AdjustVolume((sbyte)n);
			var vol = mpc.Volume();
			WriteInformation($"volume: {vol}%");
		}
	}
}
