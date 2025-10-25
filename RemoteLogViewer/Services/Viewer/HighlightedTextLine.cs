using System.Collections.Generic;

using Windows.UI;

namespace RemoteLogViewer.Services.Viewer;

public class HighlightedTextLine {
	public required long LineNumber {
		get;
		init;
	}

	public required IReadOnlyList<StyledText> StyledTexts {
		get;
		init;
	}

	public required TextStyle LineStyle {
		get;
		init;
	}
}

public class StyledText {
	public required TextStyle Style {
		get;
		init;
	}

	public required string Text {
		get;
		init;
	}
}

public class TextStyle {
	public Color? ForeColor {
		get; init;
	}
	public Color? BackColor {
		get; init;
	}
	public static readonly TextStyle Default = new();
}