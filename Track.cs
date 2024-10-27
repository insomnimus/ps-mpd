namespace MPD;

using System.Text;
using System.Collections.ObjectModel;

public class Track {
	public string Title { get; internal set; } = "";
	public string Artist { get; internal set; } = "";
	public string Album { get; internal set; } = "";
	public TimeSpan Duration { get; internal set; } = TimeSpan.Zero;
	public int TrackNo { get; internal set; } = -1;
	public int Disc { get; internal set; } = -1;
#nullable enable
	public DateTime? Date { get; set; }
	public DateTime? OriginalDate { get; set; }
#nullable disable
	public string MBTrackID { get; internal set; } = "";
	public string MBArtistID { get; internal set; } = "";
	public string MBAlbumID { get; internal set; } = "";
	public string MBReleaseTrackID { get; internal set; } = "";
	public string File { get; internal set; } = "";

	internal string NormTitle = "";
	internal string NormArtist = "";
	internal string NormAlbum = "";

	internal Track() { }
	internal Track(string file) => this.File = file;

	public override string ToString() {
		var title = string.IsNullOrEmpty(this.Title) ? "?" : this.Title;
		var album = string.IsNullOrEmpty(this.Album) ? "?" : this.Album;
		var artist = string.IsNullOrEmpty(this.Artist) ? "?" : this.Artist;
		return $"{title} [{album}] by {artist}";
	}

	internal void Eat(string key, string val) {
		switch (key) {
			case "Title":
				this.Title = val;
				this.NormTitle = val.Normal();
				break;
			case "Artist":
				this.Artist = val;
				this.NormArtist = val.Normal();
				break;
			case "Album":
				this.Album = val;
				this.NormAlbum = val.Normal();
				break;
			case "Track":
				this.TrackNo = val.ParseIntOr(-1);
				break;
			case "duration":
				var d = 0.0d;
				if (double.TryParse(val, out d)) {
					var ticks = d * 10000000.0d;
					this.Duration = new TimeSpan((long)ticks);
				}
				break;
			case "Date":
				DateTime date;
				if (DateTime.TryParse(val, out date)) {
					this.Date = date;
				}
				break;
			case "OriginalDate":
				DateTime od;
				if (DateTime.TryParse(val, out od)) {
					this.OriginalDate = od;
				}
				break;
			case "Disc":
				var disc = 0;
				if (int.TryParse(val, out disc)) {
					this.Disc = disc;
				}
				break;
			case "MUSICBRAINZ_ARTISTID":
				this.MBArtistID = val;
				break;
			case "MUSICBRAINZ_ALBUMID":
				this.MBAlbumID = val;
				break;
			case "MUSICBRAINZ_TRACKID":
				this.MBTrackID = val;
				break;
			case "MUSICBRAINZ_RELEASETRACKID":
				this.MBReleaseTrackID = val;
				break;
			case "file":
				this.File = val;
				break;
			default: break;
		}
	}
}

public class Album {
	public string Title { get; internal set; }
	public string Artist { get; internal set; }
	public List<Track> Tracks { get; internal set; }

	internal Album(List<Track> tracks) {
		tracks.Sort((a, b) => a.TrackNo.CompareTo(b.TrackNo));
		// This is internal so we can assume `tracks` isn't empty.
		this.Title = tracks[0].Album;
		this.Artist = tracks[0].Artist;
		this.Tracks = tracks;
	}
}

public class Artist {
	public string Name { get; internal set; }
	public List<Album> Albums { get; internal set; }
}

public class Playlist {
	public string Name { get; internal set; }
	public string Path { get; internal set; }
	public ReadOnlyCollection<Track> Tracks => this._tracks.AsReadOnly();

	internal string NormName;
	public DateTime ModTime { get; internal set; }
	internal List<Track> _tracks;

	internal Playlist(FileInfo f) {
		f.Refresh();

		this.Name = System.IO.Path.GetFileNameWithoutExtension(f.Name);
		this.NormName = this.Name.Normal();
		this.Path = f.FullName;
		this.ModTime = f.LastWriteTimeUtc;
		this._tracks = new();
	}

	public Track this[int index] => this._tracks[index];

	internal bool Add(Track t, bool force) {
		var add = force || !this._tracks.Any(x => x.File == t.File);
		if (add) {
			this._tracks.Add(t);
		}
		return add;
	}

	internal string Serialize() {
		var buf = new StringBuilder(1024);
		foreach (var t in this._tracks) {
			buf.AppendLine(t.File);
		}
		return buf.ToString();
	}

	internal bool IsSynced() {
		var modtime = File.GetLastWriteTimeUtc(this.Path);
		return modtime == this.ModTime;
	}

	internal bool Save(bool force) {
		if (force || this.IsSynced()) {
			File.WriteAllText(this.Path, this.Serialize(), Encoding.UTF8);
			this.ModTime = File.GetLastWriteTimeUtc(this.Path);
			return true;
		}

		return false;
	}
}
