namespace MPD;

public enum OnOffState {
	Off,
	On,
}
[Cmdlet(VerbsCommon.Set, "MpdOption")]
public class SetMpdOption: MpdCmdlet {
#nullable enable
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set consume")]
	public TriState? Consume { get; set; }
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set random")]
	public OnOffState? Random { get; set; }
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set repeat mode")]
	public OnOffState? Repeat { get; set; }
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set single state. When single is activated, playback is stopped after current song, or the song is repeated if the ‘repeat’ mode is enabled")]
	public TriState? Single { get; set; }
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set the mixramp threshold in decibels")]
	public double? MixRampDB { get; set; }
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set replay gain mode")]
	public ReplayGain? ReplayGain { get; set; }
	[Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Set crossfade in seconds")]
	public uint? CrossFade { get; set; }
#nullable disable
	protected override void ProcessRecord() {
		if (this.Consume is null && this.Random is null && this.Repeat is null && this.Single is null && this.MixRampDB is null && this.ReplayGain is null && this.CrossFade is null) {
			return;
		}
		using var mpc = this.Connect();
		if (this.Consume is TriState consume) mpc.SetConsume(consume);
		if (this.Random is OnOffState random) mpc.SetRandom(random == OnOffState.On);
		if (this.Repeat is OnOffState repeat) mpc.SetRepeat(repeat == OnOffState.On);
		if (this.Single is TriState single) mpc.SetSingle(single);
		if (this.MixRampDB is double mixramp) mpc.SetMixRamp(mixramp);
		if (this.ReplayGain is ReplayGain rg) mpc.SetReplayGain(rg);
		if (this.CrossFade is uint cf) mpc.SetCrossFade(cf);
	}
}
