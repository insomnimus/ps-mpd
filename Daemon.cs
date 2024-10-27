namespace MPD;

using System.Threading;
using System.Collections.ObjectModel;

internal class Mpd {
	internal static Mpd Instance = new();
	internal static MpdOptions Options = new MpdOptions();
	private static CancellationTokenSource lastCts = null;
	private static ReaderWriterLockSlim rwLock = new();

	private Dictionary<string, Track> files = new();
	private List<Track> tracks = new();
	private List<Playlist> playlists = new();

	private Mpd() { }

	public static Dictionary<string, List<string>> Sync<I>(MpdOptions opts, I m3uFiles, CancellationToken cancel)
	where I : IEnumerable<FileInfo> {
		if (lastCts is not null) {
			// Try to cancel any ongoing BackgroundSync
			lastCts.Cancel();
			lastCts = null;
		}

		Mpd.Options = opts;
		using var mpc = new MpcContext(opts.Connect(cancel), cancel);

		Instance.tracks.Clear();
		Instance.files.Clear();
		Instance.playlists.Clear();

		foreach (var t in mpc.ListAll()) {
			Instance.tracks.Add(t);
			Instance.files[t.File] = t;
		}

		var notFound = new Dictionary<string, List<string>>();
		foreach (var f in m3uFiles) {
			var pl = new Playlist(f);
			var lines = File.ReadAllLines(f.FullName).Select(s => s.Trim()).Where(s => s != "" && !s.StartsWith('#'));

			foreach (var s in lines) {
				Track t;
				if (Instance.files.TryGetValue(s, out t)) {
					pl._tracks.Add(t);
				} else {
					List<string> fileList;
					if (notFound.TryGetValue(f.FullName, out fileList)) {
						fileList.Add(s);
					} else {
						notFound.Add(f.FullName, [s]);
					}
				}
			}

			Instance.playlists.Add(pl);
		}

		return notFound;
	}

	public static Task BackgroundSync<I>(MpdOptions opts, I m3uFiles, CancellationTokenSource cts) where I : IEnumerable<FileInfo> {
		// Try to cancel any ongoing BackgroundSync
		if (lastCts is not null) {
			lastCts.Cancel();
			lastCts = cts;
		}
		var cancel = cts.Token;
		Mpd.Options = opts;

		return Task.Run(async () => {
			var tracks = new List<Track>();
			var files = new Dictionary<string, Track>();
			var playlists = new List<Playlist>();

			using (var mpc = new MpcContext(await opts.ConnectAsync(cancel), cancel)) {
				await foreach (var t in mpc.ListAllAsync()) {
					tracks.Add(t);
					files[t.File] = t;
				}
			}

			foreach (var f in m3uFiles) {
				var pl = new Playlist(f);
				var lines = (await File.ReadAllLinesAsync(f.FullName, cancel)).Select(s => s.Trim()).Where(s => s != "" && !s.StartsWith('#'));

				foreach (var s in lines) {
					Track t;
					if (files.TryGetValue(s, out t)) {
						pl._tracks.Add(t);
					}
				}

				playlists.Add(pl);
			}

			while (true) {
				if (cancel.IsCancellationRequested) {
					return;
				}
				if (rwLock.TryEnterWriteLock(50)) {
					Instance.files = files;
					Instance.tracks = tracks;
					Instance.playlists = playlists;
					rwLock.ExitWriteLock();
					lastCts = null;
					return;
				}
			}
		});
	}

	public static Album[] FindAlbums(TrackFilter filter, CancellationToken cancel) {
		var albums = new SortedDictionary<(string title, string artist), List<Track>>();

		foreach (var t in GetTracks(cancel)) {
			if (filter.IsMatch(t)) {
				var key = (title: t.NormAlbum, artist: t.NormArtist);
				List<Track> alb;
				if (albums.Count > 0 && albums.TryGetValue(key, out alb)) {
					alb.Add(t);
				} else {
					albums.Add(key, [t]);
				}
			}
		}

		return albums.Values.Select(alb => new Album(alb)).ToArray();
	}

	public static Artist[] FindArtists(Pattern[] patterns, CancellationToken cancel) {
		var artists = new SortedDictionary<string, SortedDictionary<string, List<Track>>>();

		foreach (var t in GetTracks(cancel)) {
			if (patterns.Length == 0 || patterns.Any(p => p.IsMatch(t.NormArtist))) {
				SortedDictionary<string, List<Track>> albs;
				if (artists.Count > 0 && artists.TryGetValue(t.NormArtist, out albs)) {
					List<Track> tracks;
					if (albs.Count > 0 && albs.TryGetValue(t.NormAlbum, out tracks)) {
						tracks.Add(t);
					} else {
						albs.Add(t.NormAlbum, [t]);
					}
					continue;
				}


				artists.Add(t.NormArtist, new SortedDictionary<string, List<Track>> { { t.NormAlbum, [t] } });
			}
		}

		return artists.Values.Select(albums => {
			var artistAlbums = new List<Album>(albums.Count);

			foreach (var album in albums.Values) {
				artistAlbums.Add(new Album(album));
			}

			return new Artist {
				Name = artistAlbums[0].Artist,
				Albums = artistAlbums,
			};
		})
		.ToArray();
	}

	public static IEnumerable<Playlist> FindPlaylists(Pattern[] patterns, CancellationToken cancel) {
		return GetPlaylists(cancel).Where(pl => patterns.Any(p => p.IsMatch(pl.NormName)));
	}

	public static IEnumerable<Track> GetTracks(CancellationToken cancel) {
		while (true) {
			if (cancel.IsCancellationRequested) {
				yield break;
			}
			if (rwLock.TryEnterReadLock(20)) {
				break;
			}
		}

		try {
			foreach (var t in Instance.tracks) {
				yield return t;
			}
		} finally {
			rwLock.ExitReadLock();
		}
	}

	public static IEnumerable<Playlist> GetPlaylists(CancellationToken cancel) {
		while (true) {
			if (cancel.IsCancellationRequested) {
				yield break;
			}
			if (rwLock.TryEnterReadLock(20)) {
				break;
			}
		}

		try {
			foreach (var pl in Instance.playlists) {
				yield return pl;
			}
		} finally {
			rwLock.ExitReadLock();
		}
	}
}
