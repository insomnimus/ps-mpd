namespace MPD;

[Cmdlet(VerbsCommon.Set, "MPDOutputAttribute", DefaultParameterSetName = "name")]
public class SetMpdOutputAttribute: MpdCmdlet, IDisposable {
	[Parameter(
		Mandatory = true,
	Position = 0,
	ParameterSetName = "name",
	HelpMessage = "Name of the output"
	)]
	[SupportsWildcards()]
	public string[] Name { get; set; } = [];
	[Parameter(
		Mandatory = true,
	ParameterSetName = "literal-name",
	HelpMessage = "Exact name of the output (case sensitive, no wildcards)"
	)]
	[Alias("ln")]
	public string[] LiteralName { get; set; } = [];
	[Parameter(
		Mandatory = true,
	ParameterSetName = "id",
	HelpMessage = "The ID of the output"
	)]
	[ValidateRange(0, int.MaxValue)]
	public int[] ID { get; set; } = [];
	[Parameter(
	Mandatory = true,
	ValueFromPipeline = true,
	ParameterSetName = "object",
	HelpMessage = "The MpdOutput object"
	)]
	public MpdOutput[] InputObject { get; set; } = [];
	[Parameter(
		Mandatory = true,
		Position = 1,
	HelpMessage = "Name of the attribute"
	)]
	public string Attribute { get; set; }
	[Parameter(
		Mandatory = true,
		Position = 2,
	HelpMessage = "Value of the attribute"
	)]
	public object Value { get; set; }
	private WildcardPattern[] nameGlobs = [];
	private List<MpdOutput> outputs = [];
	private List<MpdOutput> selectedOutputs = new();
	private MpcContext mpc;
	public void Dispose() => this.mpc?.Dispose();
	protected override void BeginProcessing() {
		this.nameGlobs = this.Name.Select(s => new WildcardPattern(s, WildcardOptions.Compiled | WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase)).ToArray();
		this.mpc = this.Connect();
		this.outputs = mpc.Outputs();
	}
	protected override void ProcessRecord() {
		foreach (var o in this.outputs) {
			var ok =
			this.ID.Any(n => o.ID == n)
			|| this.LiteralName.Any(s => o.Name == s)
			|| this.nameGlobs.Any(g => g.IsMatch(o.Name))
			|| this.InputObject.Any(x => x.Name == o.Name);
			if (ok) {
				this.selectedOutputs.Add(o);
			}
		}
	}
	protected override void EndProcessing() {
		if (this.selectedOutputs.Count == 0) {
			this.mpc?.Dispose();
			throw new Exception("No output found matching any of the provided filters");
		}
		var val = "";
		if (this.Value is Array arr) {
			if (arr.Length == 0) this.Value = false;
			else if (arr.Length == 1) this.Value = arr.GetValue(0);
		}
		if (this.Value is bool yes) val = yes ? "1" : "0";
		else if (this.Value is null) val = "0";
		else val = this.Value.ToString() ?? "";
		val = val.ToLower() switch {
			"yes" or "on" or "true" => "1",
			"no" or "off" or "false" => "0",
			_ => val,
		};
		this.mpc.SetOutputAttributes(
			this.selectedOutputs.Where(o => o.ID >= 0).Select(o => (uint)o.ID),
			this.Attribute,
			val
		);
		foreach (var o in this.selectedOutputs) {
			WriteInformation($"Set attribute '{this.Attribute}' to {val} (output {o.Name})");
		}
		this.mpc.Dispose();
	}
}
