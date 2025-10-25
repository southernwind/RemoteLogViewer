using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.UI;

using RemoteLogViewer.Models.Ssh.FileViewer;
using RemoteLogViewer.Stores.Settings;
using RemoteLogViewer.Stores.Settings.Model;

namespace RemoteLogViewer.Services.Viewer;

[AddScoped]
public class HighlightService {
	private readonly SettingsStoreModel _settingsStoreModel;
	private readonly Dictionary<(string pattern, bool ignoreCase), Regex> _regexCache = [];
	public HighlightService(SettingsStoreModel settingsStoreModel) {
		this._settingsStoreModel = settingsStoreModel;
	}

	private Regex? GetCachedRegex(string pattern, bool ignoreCase) {
		var key = (pattern, ignoreCase);
		if (this._regexCache.TryGetValue(key, out var rx)) {
			return rx;
		}
		try {
			rx = new Regex(pattern, (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) | RegexOptions.Compiled);
			this._regexCache[key] = rx;
			return rx;
		} catch { return null; }
	}

	public HighlightedTextLine HighlightText(TextLine textLine) {
		var lineContent = textLine.Content ?? string.Empty;
		if (lineContent.Length == 0) {
			return new HighlightedTextLine {
				LineNumber = textLine.LineNumber,
				StyledTexts = [new StyledText { Text = string.Empty, Style = TextStyle.Default }],
				LineStyle = TextStyle.Default
			};
		}

		var segments = new List<HighlightSegment>();
		foreach (var rule in this._settingsStoreModel.SettingsModel.HighlightSettings.Rules) {
			foreach (var condition in rule.Conditions) {
				var pattern = condition.Pattern.Value;
				if (string.IsNullOrWhiteSpace(pattern)) {
					continue;
				}
				if (condition.PatternType.Value == HighlightPatternType.Regex) {
					var regex = this.GetCachedRegex(pattern, condition.IgnoreCase.Value);
					if (regex == null) {
						continue;
					}
					foreach (Match m in regex.Matches(lineContent)) {
						if (!m.Success || m.Length == 0) {
							continue;
						}
						segments.Add(new HighlightSegment(m.Index, m.Length, new TextStyle {
							ForeColor = condition.ForeColor.Value,
							BackColor = condition.BackColor.Value
						}, condition.HighlightOnlyMatch.Value));
					}
				} else { // Exact
					var comparison = condition.IgnoreCase.Value ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
					var searchStart = 0;
					while (searchStart < lineContent.Length) {
						var idx = lineContent.IndexOf(pattern, searchStart, comparison);
						if (idx < 0) {
							break;
						}
						segments.Add(new HighlightSegment(idx, pattern.Length, new TextStyle {
							ForeColor = condition.ForeColor.Value,
							BackColor = condition.BackColor.Value
						}, condition.HighlightOnlyMatch.Value));
						searchStart = idx + pattern.Length;
					}
				}
			}
		}

		if (segments.Count == 0) {
			return new HighlightedTextLine {
				LineNumber = textLine.LineNumber,
				StyledTexts = [new StyledText { Text = lineContent, Style = TextStyle.Default }],
				LineStyle = TextStyle.Default
			};
		}

		var lineStyle = segments.FirstOrDefault(x => !x.OnlyMatch)?.Style;
		segments = segments.Where(x => x.OnlyMatch).OrderBy(s => s.Start).ThenByDescending(s => s.Length).ToList();
		var styledList = new List<StyledText>();
		var cursor = 0;
		foreach (var seg in segments) {
			if (seg.Start > cursor) {
				styledList.Add(new StyledText { Text = lineContent[cursor..seg.Start], Style = TextStyle.Default });
			}
			var start = Math.Max(cursor, seg.Start);
			var length = seg.Length - (start - seg.Start);
			styledList.Add(new StyledText { Text = lineContent.Substring(start, length), Style = seg.Style });
			cursor = seg.Start + seg.Length;
		}
		if (cursor < lineContent.Length) {
			styledList.Add(new StyledText { Text = lineContent[cursor..], Style = TextStyle.Default });
		}
		return new HighlightedTextLine { LineNumber = textLine.LineNumber, StyledTexts = styledList, LineStyle = lineStyle ?? TextStyle.Default };
	}

	private record HighlightSegment(int Start, int Length, TextStyle Style, bool OnlyMatch);
}
