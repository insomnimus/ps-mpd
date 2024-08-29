namespace MPD;

using System.Globalization;
using System.Text;

public static class StringExt {
	public static string Normal(this string s) {
		var buf = new StringBuilder(s.Length);

		foreach (var c in s.Normalize(NormalizationForm.FormD).EnumerateRunes()) {
			var cat = Rune.GetUnicodeCategory(c);
			if (cat != UnicodeCategory.NonSpacingMark) {
				buf.Append(Rune.ToUpperInvariant(c));
			}
		}

		return buf.ToString();
	}

	public static int ParseIntOr(this string s, int _default) {
		var n = 0;
		if (!int.TryParse(s, out n)) return _default;
		else return n;
	}

	public static string Plural(this string s, long n) {
		if (n == 1) return $"1 {s}";
		else return $"{n} {s}s";
	}
}

public static class DateTimeExt {
	public static bool IsEpoch(this DateTime dt) {
		return dt.ToFileTime() == 0;
	}
}

public static class ListExt {
	public static void Truncate<T>(this List<T> l, int newLength) {
		if (l.Count > newLength) {
			l.RemoveRange(newLength, l.Count - newLength);
		}
	}
}
