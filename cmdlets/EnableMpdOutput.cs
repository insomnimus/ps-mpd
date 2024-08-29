namespace MPD;

[Cmdlet(VerbsLifecycle.Enable, "MPDOutput", DefaultParameterSetName = "name")]
public class EnableMpdOutput: MpdCmdlet, IDisposable {
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
		this.mpc.EnableOutputs(this.selectedOutputs.Where(o => o.ID >= 0).Select(o => (uint)o.ID));
		foreach (var o in this.selectedOutputs) {
			WriteInformation($"Enabled output {o.Name}");
		}
		this.mpc.Dispose();
	}
}
