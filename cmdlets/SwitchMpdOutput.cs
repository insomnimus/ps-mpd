namespace MPD;

[Cmdlet(VerbsCommon.Switch, "MPDOutput", DefaultParameterSetName = "name")]
public class SwitchMpdOutput: MpdCmdlet {
	[Parameter(
	Mandatory = true,
	Position = 0,
	ParameterSetName = "name",
	HelpMessage = "Name of the output"
	)]
	[SupportsWildcards()]
	public string Name { get; set; }
	[Parameter(
		Mandatory = true,
	ParameterSetName = "literal-name",
	HelpMessage = "Exact name of the output (case sensitive, no wildcards)"
	)]
	[Alias("ln")]
	public string LiteralName { get; set; }
	[Parameter(
		Mandatory = true,
	ParameterSetName = "id",
	HelpMessage = "The ID of the output"
	)]
	[ValidateRange(0, int.MaxValue)]
	public int ID { get; set; } = -1;
	[Parameter(
		Mandatory = true,
		ValueFromPipeline = true,
		ParameterSetName = "object",
		HelpMessage = "The MpdOutput object"
	)]
	public MpdOutput InputObject { get; set; }
	protected override void EndProcessing() {
		using var mpc = this.Connect();
		if (this.InputObject is not null) {
			this.LiteralName = this.InputObject.Name;
		}
		WildcardPattern glob = string.IsNullOrEmpty(this.Name) ? null : new(this.Name, WildcardOptions.Compiled | WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);
		var o = mpc.Outputs().Find(o => {
			return (this.ID >= 0 && o.ID == this.ID)
			 || (glob is not null && glob.IsMatch(o.Name))
			|| (!string.IsNullOrEmpty(this.LiteralName) && o.Name == this.LiteralName);
		});
		if (o is null) {
			throw new Exception("Could not find an output device matching the provided filter");
		}
		mpc.MoveOutput(o.Name);
		WriteInformation($"Assigned output {o.Name} to the current partition");
	}
}
