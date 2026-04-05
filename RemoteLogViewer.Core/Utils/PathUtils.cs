using System.IO;

using RemoteLogViewer.Core.Services.Ssh;

namespace RemoteLogViewer.Core.Utils;

/// <summary>
/// パスに関するユーティリティクラスです。
/// </summary>
public static class PathUtils {
	/// <summary>
	/// Unixパスを結合します。
	/// </summary>
	/// <param name="path1">ベースとなるパス。</param>
	/// <param name="path2">結合するパス。</param>
	/// <param name="fsoType">ファイルシステムのオブジェクト種別。</param>
	/// <returns>結合されたパス。</returns>
	public static string CombineUnixPath(string path1, string path2, FileSystemObjectType fsoType) {
		if (path2.StartsWith('/')) {
			return path2;
		}
		return path1.TrimEnd('/') + "/" + path2 + (fsoType == FileSystemObjectType.Directory || fsoType == FileSystemObjectType.SymlinkDirectory ? "/" : "");
	}

	/// <summary>
	/// パスからファイル名またはディレクトリ名を取得します。
	/// </summary>
	/// <param name="path">対象のパス。</param>
	/// <returns>ファイル名またはディレクトリ名。</returns>
	public static string GetFileOrDirectoryName(string path) {
		return Path.GetFileName(path.TrimEnd('/'));
	}
}