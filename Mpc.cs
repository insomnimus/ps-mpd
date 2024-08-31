namespace MPD;

using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

internal struct MpdOptions {
	internal IPEndPoint Endpoint;
	internal string Password;

	public MpdOptions() {
		this.Endpoint = new(IPAddress.Loopback, 6600);
		this.Password = "";
	}

	internal MpdOptions(IPAddress ip, ushort port = 6600) {
		this.Endpoint = new(ip, port);
		this.Password = "";
	}

	internal Mpc Connect(CancellationToken cancel) {
		return Mpc.Connect(this.Endpoint, this.Password, cancel);
	}

	internal ValueTask<Mpc> ConnectAsync(CancellationToken cancel) {
		return Mpc.ConnectAsync(this.Endpoint, this.Password, cancel);
	}
}

public enum PlaybackState {
	Playing,
	Stopped,
	Paused,
}

public struct MpdStatus {
#nullable enable
	public Track Track;
	public TimeSpan? Elapsed;
	public sbyte? Volume;
	public bool? Repeat;
	public bool? Random;
	public TriState? Single;
	public TriState? Consume;
	public PlaybackState? State;
	public double? MixRampDB;
	public ReplayGain? ReplayGain;
	public uint CrossFade;
#nullable disable

	public MpdStatus() {
		this.Track = null;
		this.Volume = -1;
		this.CrossFade = 0;
	}

	internal void Eat(string key, string val) {
		switch (key) {
			case "volume":
				sbyte n = -1;
				if (sbyte.TryParse(val, out n)) {
					this.Volume = n;
				}
				break;
			case "repeat":
				this.Repeat = val switch {
					"0" => false,
					"1" => true,
					_ => null,
				};
				break;
			case "random":
				this.Random = val switch {
					"0" => false,
					"1" => true,
					_ => null,
				};
				break;
			case "single":
				this.Single = val switch {
					"0" => TriState.Off,
					"1" => TriState.On,
					"oneshot" => TriState.OneShot,
					_ => null,
				};
				break;
			case "consume":
				this.Consume = val switch {
					"0" => TriState.Off,
					"1" => TriState.On,
					"oneshot" => TriState.OneShot,
					_ => null,
				};
				break;
			case "state":
				this.State = val switch {
					"play" => PlaybackState.Playing,
					"stop" => PlaybackState.Stopped,
					"pause" => PlaybackState.Paused,
					_ => null,
				};
				break;
			case "mixrampdb":
				double db = 0.0;
				if (double.TryParse(val, out db)) {
					this.MixRampDB = db;
				}
				break;
			case "elapsed":
				double d = 0.0;
				if (double.TryParse(val, out d)) {
					var ticks = d * 10000000;
					this.Elapsed = new((long)ticks);
				}
				break;
			case "xfade":
				var cf = 0u;
				if (uint.TryParse(val, out cf)) {
					this.CrossFade = cf;
				}
				break;

			default: break;
		}
	}
}

public class ProtocolException: Exception {
	public ProtocolException(string msg) : base(msg) { }
}

public enum TriState {
	Off,
	On,
	OneShot,
}

public enum ReplayGain {
	Off,
	Track, Album,
	Auto,
}

public struct MpdMount {
	public string Path { get; internal set; }
}

public class MpdOutput {
	public int ID { get; internal set; } = -1;
	public string Name { get; internal set; } = "";
	public bool Enabled { get; internal set; }
	public IReadOnlyDictionary<string, string> Attributes => this._attributes;
	public string Plugin { get; internal set; }

	internal Dictionary<string, string> _attributes = new();

	public MpdOutput() { }

	internal void Eat(string key, string val) {
		switch (key) {
			case "outputname":
				this.Name = val;
				break;
			case "plugin":
				this.Plugin = val;
				break;
			case "outputenabled":
				this.Enabled = val == "1";
				break;
			case "outputid":
				this.ID = val.ParseIntOr(-1);
				break;
			case "attribute":
				var eqPos = val.IndexOf('=');
				if (eqPos < 0) {
					this._attributes[val] = "";
				} else {
					this._attributes[val.Substring(0, eqPos)] = val.Substring(eqPos + 1);
				}
				break;

			default: break;
		}
	}
}

internal class Mpc: IDisposable {
	static byte[] TAGTYPES_CLEAR = [116, 97, 103, 116, 121, 112, 101, 115, 32, 99, 108, 101, 97, 114]; // tagtypes clear
																									   // tagtypes enable Artist Album Title Track Date OriginalDate Disc MUSICBRAINZ_ARTISTID MUSICBRAINZ_ALBUMID MUSICBRAINZ_TRACKID MUSICBRAINZ_RELEASETRACKID
	static byte[] TAGTYPES_ENABLE = [116, 97, 103, 116, 121, 112, 101, 115, 32, 101, 110, 97, 98, 108, 101, 32, 65, 114, 116, 105, 115, 116, 32, 65, 108, 98, 117, 109, 32, 84, 105, 116, 108, 101, 32, 84, 114, 97, 99, 107, 32, 68, 97, 116, 101, 32, 79, 114, 105, 103, 105, 110, 97, 108, 68, 97, 116, 101, 32, 68, 105, 115, 99, 32, 77, 85, 83, 73, 67, 66, 82, 65, 73, 78, 90, 95, 65, 82, 84, 73, 83, 84, 73, 68, 32, 77, 85, 83, 73, 67, 66, 82, 65, 73, 78, 90, 95, 65, 76, 66, 85, 77, 73, 68, 32, 77, 85, 83, 73, 67, 66, 82, 65, 73, 78, 90, 95, 84, 82, 65, 67, 75, 73, 68, 32, 77, 85, 83, 73, 67, 66, 82, 65, 73, 78, 90, 95, 82, 69, 76, 69, 65, 83, 69, 84, 82, 65, 67, 75, 73, 68];
	static byte[] COMMAND_LIST_BEGIN = [99, 111, 109, 109, 97, 110, 100, 95, 108, 105, 115, 116, 95, 98, 101, 103, 105, 110]; // command_list_begin
	static byte[] COMMAND_LIST_END = [99, 111, 109, 109, 97, 110, 100, 95, 108, 105, 115, 116, 95, 101, 110, 100]; // command_list_end

	private TcpConnection tcp;

	private Mpc(TcpConnection tcp) => this.tcp = tcp;

	public void Dispose() => this.tcp.Dispose();

	public static string EncodeValue(string s) {
		var buf = new StringBuilder(s.Length + 2);
		buf.Append('"');
		foreach (var c in s) {
			if (c == '\'' || c == '"' || c == '\\') {
				buf.Append('\\');
			}
			buf.Append(c);
		}

		buf.Append('"');
		return buf.ToString();
	}

	public static string EncodeTriState(TriState s) => s switch {
		TriState.Off => "0",
		TriState.On => "1",
		TriState.OneShot => "oneshot",
		_ => throw new Exception($"invalid TriState value {s}"),
	};

	public static async ValueTask<Mpc> ConnectAsync(IPEndPoint endpoint, string password, CancellationToken cancel) {
		var tcp = await TcpConnection.Connect(endpoint, cancel);
		// Upon connecting, the server will send a line in the form "OK MPD <version>"
		var s = await tcp.ReadLine(cancel);
		if (s is null) {
			throw new Exception("The server closed the connection");
		} else if (!s.StartsWith("OK MPD ")) {
			throw new Exception("The server did not identify itself as MPD");
		}

		var mpc = new Mpc(tcp);
		if (!string.IsNullOrEmpty(password)) {
			await mpc.ExecuteAsync($"password {EncodeValue(password)}", cancel);
		}

		// Tell MPD not to show tags we don't use, for performance.
		await mpc.ExecuteManyAsync([TAGTYPES_CLEAR, TAGTYPES_ENABLE], cancel);
		return mpc;
	}

	public static Mpc Connect(IPEndPoint endpoint, string password, CancellationToken cancel) {
		return ConnectAsync(endpoint, password, cancel).GetAwaiter().GetResult();
	}

	// Warning: The caller needs to exhaust the returned enumerable; else there will be junk to be read for the next command.
	public async IAsyncEnumerable<(string key, string val)> RunIter(byte[] cmd, [EnumeratorCancellation] CancellationToken cancel = default) {
		await this.tcp.WriteLine(cmd, cancel);
		while (true) {
			var s = await this.tcp.ReadLine(cancel);
			if (s is null || s == "OK") {
				break;
			} else if (s.StartsWith("ACK")) {
				throw new ProtocolException(s);
			}


			var colonPos = s.IndexOf(": ");
			if (colonPos < 0) {
				yield break;
			}
			yield return (s.Substring(0, colonPos), s.Substring(colonPos + 2));
		}
	}

	// Warning: The caller needs to exhaust the returned enumerable; else there will be junk to be read for the next command.
	public IAsyncEnumerable<(string key, string val)> RunIter(string cmd, CancellationToken cancel) {
		return this.RunIter(Encoding.UTF8.GetBytes(cmd), cancel);
	}

	public async ValueTask<List<(string key, string val)>> Run(byte[] cmd, CancellationToken cancel) {
		var list = new List<(string key, string val)>();
		await foreach (var entry in this.RunIter(cmd, cancel)) {
			list.Add(entry);
		}
		return list;
	}

	public ValueTask<List<(string key, string val)>> Run(string cmd, CancellationToken cancel) {
		return this.Run(Encoding.UTF8.GetBytes(cmd), cancel);
	}

	public async ValueTask ExecuteAsync(byte[] cmd, CancellationToken cancel) {
		await foreach (var _ in this.RunIter(cmd, cancel).WithCancellation(cancel)) {
			// Ignore
		}
	}

	public async ValueTask ExecuteAsync(string cmd, CancellationToken cancel) {
		await foreach (var _ in this.RunIter(cmd, cancel).WithCancellation(cancel)) {
			// Ignore
		}
	}

	public async ValueTask ExecuteManyAsync<I>(I commands, CancellationToken cancel) where I : IEnumerable<string> {
		await this.tcp.WriteLine(COMMAND_LIST_BEGIN, cancel);
		// Todo: Maybe buffer writes
		foreach (var cmd in commands) {
			await this.tcp.WriteLine(cmd, Encoding.UTF8, cancel);
		}
		await this.ExecuteAsync(COMMAND_LIST_END, cancel);
	}

	public async ValueTask ExecuteManyAsync(IEnumerable<byte[]> commands, CancellationToken cancel) {
		await this.tcp.WriteLine(COMMAND_LIST_BEGIN, cancel);
		// Todo: Maybe buffer writes
		foreach (var cmd in commands) {
			await this.tcp.WriteLine(cmd, cancel);
		}
		await this.ExecuteAsync(COMMAND_LIST_END, cancel);
	}
}

internal class MpcContext: IDisposable {
	// Constants {
	static byte[] CLEAR = [99, 108, 101, 97, 114]; // clear
	static byte[] CURRENTSONG = [99, 117, 114, 114, 101, 110, 116, 115, 111, 110, 103]; // currentsong
	static byte[] GETVOL = [103, 101, 116, 118, 111, 108]; // getvol
	static byte[] LISTALLINFO = [108, 105, 115, 116, 97, 108, 108, 105, 110, 102, 111]; // listallinfo
	static byte[] NEXT = [110, 101, 120, 116]; // next
	static byte[] OUTPUTS = [111, 117, 116, 112, 117, 116, 115]; // outputs
	static byte[] PAUSE = [112, 97, 117, 115, 101, 32, 49]; // pause 1
	static byte[] PLAY = [112, 108, 97, 121]; // play
	static byte[] PLAYLISTINFO = [112, 108, 97, 121, 108, 105, 115, 116, 105, 110, 102, 111]; // playlistinfo
	static byte[] PREVIOUS = [112, 114, 101, 118, 105, 111, 117, 115]; // previous
	static byte[] REPLAY_GAIN_STATUS = [114, 101, 112, 108, 97, 121, 95, 103, 97, 105, 110, 95, 115, 116, 97, 116, 117, 115]; // replay_gain_status
	static byte[] STATS = [115, 116, 97, 116, 115]; // stats
	static byte[] STATUS = [115, 116, 97, 116, 117, 115]; // status
	static byte[] STOP = [115, 116, 111, 112]; // stop
	static byte[] TOGGLE = [112, 97, 117, 115, 101]; // pause
													 // }

	private Mpc mpc;
	private CancellationToken cancel;

	internal MpcContext(Mpc mpc, CancellationToken cancel) {
		this.mpc = mpc;
		this.cancel = cancel;
	}

	public void Dispose() => this.mpc.Dispose();

	private void execute(byte[] cmd) => this.mpc.ExecuteAsync(cmd, this.cancel).AsTask().Wait();
	private void execute(string cmd) => this.mpc.ExecuteAsync(cmd, this.cancel).AsTask().Wait();
	private void executeMany<I>(I commands) where I : IEnumerable<string> => this.mpc.ExecuteManyAsync(commands, this.cancel).AsTask().Wait();

	private IEnumerable<(string key, string val)> runIter(byte[] cmd) => this.mpc.RunIter(cmd, this.cancel).ToBlockingEnumerable();
	private IEnumerable<(string key, string val)> runIter(string cmd) => this.mpc.RunIter(cmd, this.cancel).ToBlockingEnumerable();
	private List<(string key, string val)> run(byte[] cmd) => this.mpc.Run(cmd, this.cancel).AsTask().GetAwaiter().GetResult();
	private List<(string key, string val)> run(string cmd) => this.mpc.Run(cmd, this.cancel).AsTask().GetAwaiter().GetResult();

	private static async IAsyncEnumerable<Track> parseTracksAsync<I>(I input)
	where I : IAsyncEnumerable<(string key, string val)> {
		var iter = input.GetAsyncEnumerator();
		var advance = true;

		while (true) {
			if (advance && !(await iter.MoveNextAsync())) {
				break;
			} else {
				advance = true;
			}

			var (key, val) = iter.Current;
			if (key != "file") {
				continue;
			}

			var t = new Track(val);

			while (await iter.MoveNextAsync()) {
				(key, val) = iter.Current;

				if (key == "file" || key == "directory") {
					advance = false;
					break;
				}
				t.Eat(key, val);
			}

			yield return t;
		}
	}

	private async ValueTask<Track> currentAsync() {
		var t = new Track();
		await foreach (var (key, val) in this.mpc.RunIter(CURRENTSONG, this.cancel)) {
			t.Eat(key, val);
		}

		if (string.IsNullOrEmpty(t.File)) {
			return null;
		}
		return t;
	}

	public MpdStatus Status() {
		var state = new MpdStatus();
		foreach (var (key, val) in this.runIter(STATUS)) {
			state.Eat(key, val);
		}

		foreach (var (key, val) in this.runIter(REPLAY_GAIN_STATUS)) {
			switch (key) {
				case "replay_gain_mode":
					state.ReplayGain = val switch {
						"off" => ReplayGain.Off,
						"track" => ReplayGain.Track,
						"album" => ReplayGain.Album,
						"auto" => ReplayGain.Auto,
						_ => null,
					};
					break;

				default: break;
			}
		}

		state.Track = this.Current();
		return state;
	}

	public ulong QueueLength() {
		foreach (var (key, val) in this.run(STATUS)) {
			if (key == "playlistlength") {
				var n = 0uL;
				if (ulong.TryParse(val, out n)) {
					return n;
				} else {
					return 0;
				}
			}
		}

		return 0;
	}

	public sbyte Volume() {
		foreach (var (key, val) in this.run(GETVOL)) {
			if (key == "volume") {
				sbyte n = -1;
				if (sbyte.TryParse(val, out n)) {
					return n;
				} else {
					break;
				}
			}
		}

		return -1;
	}

	public IEnumerable<Track> CurrentQueueIter() => parseTracksAsync(this.mpc.RunIter(PLAYLISTINFO, this.cancel)).ToBlockingEnumerable();
	public Track[] CurrentQueue() => this.CurrentQueueIter().ToArray();
	public Track Current() => this.currentAsync().GetAwaiter().GetResult();
	public IAsyncEnumerable<Track> ListAllAsync() => parseTracksAsync(this.mpc.RunIter(LISTALLINFO, this.cancel));
	public IEnumerable<Track> ListAll() => parseTracksAsync(this.mpc.RunIter(LISTALLINFO, this.cancel)).ToBlockingEnumerable();

	public ulong TrackCount() {
		foreach (var (key, val) in this.run(STATS)) {
			var n = 0uL;
			if (key == "songs") {
				if (ulong.TryParse(val, out n)) {
					return n;
				} else {
					return 0;
				}
			}
		}
		return 0;
	}

	public void Enqueue<I>(I tracks) where I : IEnumerable<Track> {
		this.executeMany(tracks.Select(t => $"add {Mpc.EncodeValue(t.File)}"));
	}

	public void SeekPlayback(sbyte relative, double seconds) {
		if (seconds < 0.0) {
			throw new Exception($"Internal error: negative `seconds` value passed to MpcContext.SeekPosition: {seconds}");
		}
		var s = "";
		if (relative < 0) {
			s = $"seekcur -{seconds:0.00}";
		} else if (relative == 0) {
			s = $"seekcur {seconds:0.00}";
		} else {
			s = $"seekcur +{seconds:0.00}";
		}

		this.execute(s);
	}

	public void SeekQueue(ulong idx) => this.execute($"play {idx}");
	public void SetReplayGain(ReplayGain mode) => this.execute(mode switch {
		ReplayGain.Off => "replay_gain_mode off",
		ReplayGain.Track => "replay_gain_mode track",
		ReplayGain.Album => "replay_gain_mode album",
		ReplayGain.Auto => "replay_gain_mode auto",
		_ => throw new ArgumentException($"Invalid ReplayGain mode: {mode}", "mode"),
	});

	public void Clear() => this.execute(CLEAR);
	public void Play() => this.execute(PLAY);
	public void Toggle() => this.execute(TOGGLE);
	public void Stop() => this.execute(STOP);
	public void Pause() => this.execute(PAUSE);
	public void Previous() => this.execute(PREVIOUS);
	public void Next() => this.execute(NEXT);
	public void SetVolume(byte vol) => this.execute($"setvol {vol}");
	public void AdjustVolume(sbyte amount) => this.execute($"volume {amount}");
	public void SetConsume(TriState state) => this.execute($"consume {Mpc.EncodeTriState(state)}");
	public void SetSingle(TriState state) => this.execute($"single {Mpc.EncodeTriState(state)}");
	public void SetRandom(bool enable) => this.execute(enable ? "random 1" : "random 0");
	public void SetRepeat(bool enable) => this.execute(enable ? "repeat 1" : "repeat 0");
	public void SetMixRamp(double db) => this.execute($"mixrampdb {db:0.00}");
	public void SetCrossFade(uint crossfade) => this.execute($"crossfade {crossfade}");

	// Mount and device commands
	public void Mount(string path, string uri) => this.execute($"mount {Mpc.EncodeValue(path)} {Mpc.EncodeValue(uri)}");
	public void Unmount(string path) => this.execute($"unmount {Mpc.EncodeValue(path)}");
	public List<MpdMount> Mounts() {
		throw new Exception();
	}

	public List<MpdOutput> Outputs() {
		var outputs = new List<MpdOutput>();
		var o = new MpdOutput();
		foreach (var (key, val) in this.run(OUTPUTS)) {
			if (key == "outputid") {
				if (o.ID >= 0) {
					outputs.Add(o);
					o = new();
				}
				o.ID = val.ParseIntOr(-1);
			} else {
				o.Eat(key, val);
			}
		}

		if (o.ID >= 0) {
			outputs.Add(o);
		}

		return outputs;
	}

	public void ToggleOutput(uint id) => this.execute($"toggleoutput {id}");

	public void EnableOutputs<I>(I ids) where I : IEnumerable<uint> {
		this.executeMany(ids.Select(id => $"enableoutput {id}"));
	}

	public void DisableOutputs<I>(I ids) where I : IEnumerable<uint> {
		this.executeMany(ids.Select(id => $"disableoutput {id}"));
	}

	public void SetOutputAttributes<I>(I outputIds, string attrKey, string attrValue) where I : IEnumerable<uint> {
		attrKey = Mpc.EncodeValue(attrKey);
		attrValue = Mpc.EncodeValue(attrValue);
		this.executeMany(outputIds.Select(id => $"outputset {id} {attrKey} {attrValue}"));
	}

	public void MoveOutput(string name) => this.execute($"moveoutput {Mpc.EncodeValue(name)}");
}
