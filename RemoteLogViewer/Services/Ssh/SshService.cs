using Renci.SshNet;

using System.Collections.Generic;
using System.Text;

namespace RemoteLogViewer.Services.Ssh;

/// <summary>
///     SSH 接続とコマンド実行を提供します。
/// </summary>
[AddScoped]
public class SshService : IDisposable {
	private SshClient? _client;
	public string? CSharpEncoding {
		get;
		private set;
	}

	public string? IconvEncoding {
		get {
			if (this.CSharpEncoding is null) {
				return null;
			}
			var pair = Constants.EncodingPairs.FirstOrDefault(ep => ep.CSharp == this.CSharpEncoding);
			return pair?.Iconv;
		}
	}

	/// <summary>
	///     パスワード / 鍵認証で接続します。password と privateKeyPath の両方が指定された場合は複数メソッドで試行します。
	/// </summary>
	/// <param name="host">ホスト。</param>
	/// <param name="port">ポート。</param>
	/// <param name="user">ユーザー名。</param>
	/// <param name="password">パスワード (任意)。</param>
	/// <param name="privateKeyPath">秘密鍵パス (任意)。</param>
	/// <param name="privateKeyPassphrase">秘密鍵パスフレーズ (任意)。</param>
	/// <param name="encoding">文字エンコード(CSharpのEncoding.GetEncoding()で取得可能な名称</param>
	public void Connect(string host, int port, string user, string? password, string? privateKeyPath, string? privateKeyPassphrase, string encoding) {
		this.Disconnect();

		if (!string.IsNullOrWhiteSpace(privateKeyPath)) {
			var methods = new List<AuthenticationMethod>();
			// 鍵認証
			PrivateKeyFile pkFile;
			if (!string.IsNullOrEmpty(privateKeyPassphrase)) {
				pkFile = new PrivateKeyFile(privateKeyPath, privateKeyPassphrase);
			} else {
				pkFile = new PrivateKeyFile(privateKeyPath);
			}
			methods.Add(new PrivateKeyAuthenticationMethod(user, pkFile));
			// 併用できる場合はパスワードも追加
			if (!string.IsNullOrWhiteSpace(password)) {
				methods.Add(new PasswordAuthenticationMethod(user, password));
			}
			var connectionInfo = new ConnectionInfo(host, port, user, [.. methods]);
			connectionInfo.Encoding = Encoding.GetEncoding(encoding);
			this._client = new SshClient(connectionInfo);
		} else {
			// 従来のパスワード専用
			var connectionInfo = new ConnectionInfo(host, port, user, [new PasswordAuthenticationMethod(user, password ?? string.Empty)]);
			connectionInfo.Encoding = Encoding.GetEncoding(encoding);
			this._client = new SshClient(connectionInfo);
		}

		this._client.Connect();
		this.CSharpEncoding = encoding;
	}

	/// <summary>
	///     接続を切断します。
	/// </summary>
	public void Disconnect() {
		if (this._client is { IsConnected: true }) {
			this._client.Disconnect();
		}
		this._client?.Dispose();
		this._client = null;
	}

	/// <summary>
	///     コマンドを実行し結果文字列を返します。
	/// </summary>
	/// <param name="command">コマンド。</param>
	/// <returns>標準出力。</returns>
	public string Run(string command) {
		if (this._client is not { IsConnected: true }) {
			throw new InvalidOperationException("SSH not connected.");
		}
		using var cmd = this._client.CreateCommand(command);
		return cmd.Execute();
	}

	/// <summary>
	///     リソースを解放します。
	/// </summary>
	public void Dispose() {
		this.Disconnect();
	}
}
