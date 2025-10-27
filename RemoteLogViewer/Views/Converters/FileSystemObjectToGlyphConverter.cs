using Microsoft.UI.Xaml.Data;

using RemoteLogViewer.Services.Ssh;

namespace RemoteLogViewer.Views.Converters;

/// <summary>
///     FileSystemObjectType からアイコン用グリフ文字列を返します。
/// </summary>
public sealed class FileSystemObjectTypeToGlyphConverter : IValueConverter {
	public object Convert(object value, Type targetType, object parameter, string language) {
		if (value is not FileSystemObjectType fsoType) {
			return string.Empty;
		}
		return fsoType switch {
			FileSystemObjectType.Directory => "\uED25", // フォルダー
			FileSystemObjectType.SymlinkDirectory => "\uE71B", // ディレクトリリンク
			FileSystemObjectType.SymlinkFile => "\uE71B", // ファイルリンク
			FileSystemObjectType.File => "\uE8A5", // ファイル
			_ => "\uE8A5"
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language) {
		throw new NotSupportedException();
	}
}
