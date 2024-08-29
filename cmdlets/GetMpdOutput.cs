namespace MPD;

[Cmdlet(VerbsCommon.Get, "MPDOutput", DefaultParameterSetName = "name")]
[OutputType(typeof(MpdOutput))]
public class GetMpdOutput: MpdCmdlet {
	[Parameter(Position = 0, ParameterSetName = "name", HelpMessage = "Name of the output")]
	[SupportsWildcards()]
	public string Name { get; set; }
	[Parameter(ParameterSetName = "literal-name", HelpMessage = "Exact name of the output (case sensitive, no wildcards)")]
	[Alias("ln")]
	public string LiteralName { get; set; }
	[Parameter(ParameterSetName = "id", HelpMessage = "The ID of the output")]
	[ValidateRange(0, int.MaxValue)]
	public int ID { get; set; } = -1;
	protected override void ProcessRecord() {
		using var mpc = this.Connect();
		WildcardPattern glob = string.IsNullOrEmpty(this.Name) ? null : new(this.Name, WildcardOptions.Compiled | WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);
		var all = glob is null && this.ID < 0 && string.IsNullOrEmpty(this.LiteralName);
		foreach (var o in mpc.Outputs()) {
			var ok = all ||
			(this.ID >= 0 && o.ID == this.ID)
			 || (glob is not null && glob.IsMatch(o.Name))
			|| (!string.IsNullOrEmpty(this.LiteralName) && o.Name == this.LiteralName);
			if (ok) {
				WriteObject(o);
			}
		}
	}
}
