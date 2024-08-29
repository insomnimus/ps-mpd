namespace MPD;

using System.Text;
using System.Net;
using System.Net.Sockets;

internal class TcpConnection: IDisposable {
	const byte LF = (byte)'\n';

	private TcpClient client;
	private NetworkStream stream;
	public bool DataAvailable => this.readBuf.Count > 0 || this.stream.DataAvailable;

	// Whatever excess bytes we read will be stored here
	private List<byte> readBuf = new();
	// General purpose buffer to be reused
	private byte[] buffer = new byte[4096];

	public void Dispose() => this.client.Dispose();

	public TcpConnection(TcpClient client) {
		this.client = client;
		this.stream = client.GetStream();
	}

	public static async ValueTask<TcpConnection> Connect(IPEndPoint endpoint, CancellationToken cancel) {
		var c = new TcpClient();
		await c.ConnectAsync(endpoint, cancel);
		return new TcpConnection(c);
	}

	public async ValueTask<byte[]> ReadLineOfBytes(CancellationToken cancel) {
		// We might already have a line buffered
		var lfPos = this.readBuf.IndexOf(LF);
		if (lfPos >= 0) {
			// Excludes the final \n
			var ln = new byte[lfPos];
			for (var i = 0; i < lfPos; i++) {
				ln[i] = this.readBuf[i];
			}
			this.readBuf.RemoveRange(0, lfPos + 1);
			return ln;
		}

		while (true) {
			var n = await this.stream.ReadAsync(this.buffer, 0, this.buffer.Length, cancel);
			if (n == 0) {
				if (this.readBuf.Count == 0) {
					return null;
				}
				return this.readBuf.ToArray();
			}

			this.readBuf.EnsureCapacity(this.readBuf.Count + n);

			byte[] ln = [];
			var found = false;

			for (var i = 0; i < n; i++) {
				var c = this.buffer[i];
				if (!found && c == LF) {
					ln = this.readBuf.ToArray();
					found = true;
					this.readBuf.Clear();
				} else {
					this.readBuf.Add(c);
				}
			}

			if (found) {
				return ln;
			}
		}
	}

	public async ValueTask<string> ReadLine(CancellationToken cancel) {
		var bytes = await this.ReadLineOfBytes(cancel);
		if (bytes is null) {
			return null;
		} else {
			return System.Text.Encoding.UTF8.GetString(bytes);
		}
	}

	public async Task WriteLine(byte[] bytes, CancellationToken cancel) {
		await this.stream.WriteAsync(bytes, 0, bytes.Length, cancel);
		await this.stream.WriteAsync([LF], 0, 1, cancel);
	}

	public async Task WriteLine(string s, Encoding encoding, CancellationToken cancel) {
		var bytes = encoding.GetBytes(s);
		await this.WriteLine(bytes, cancel);
	}

	public void DiscardRead(CancellationToken cancel) {
		this.readBuf.Clear();
	}
}
