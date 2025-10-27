using Microsoft.UI.Xaml.Data;
using RemoteLogViewer.Services;

namespace RemoteLogViewer.Views.Converters;

/// <summary>
/// NotificationSeverity を Segoe MDL2 Assets のグリフ文字へ変換します。
/// </summary>
public class SeverityToGlyphConverter : IValueConverter {
	public object Convert(object value, Type targetType, object parameter, string language) {
		if (value is NotificationSeverity sev) {
			return sev switch {
				NotificationSeverity.Info => "\uE946",      // Info icon
				NotificationSeverity.Warning => "\uE7BA",   // Warning icon
				NotificationSeverity.Error => "\uEA39",     // Error badge
				NotificationSeverity.Critical => "\uE814",  // Critical/Stop
				_ => "\uE946"
			};
		}
		return "";
	}
	public object ConvertBack(object value, Type targetType, object parameter, string language) {
		throw new NotSupportedException();
	}
}
