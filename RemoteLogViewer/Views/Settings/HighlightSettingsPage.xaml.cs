using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using RemoteLogViewer.ViewModels.Settings.Highlight;

namespace RemoteLogViewer.Views.Settings;

public sealed partial class HighlightSettingsPage : Page {
	public HighlightSettingsPageViewModel? ViewModel {
		get;
		private set;
	}

	public HighlightSettingsPage() {
		this.InitializeComponent();
	}

	/// <summary>
	/// ナビゲート時に ViewModel を受け取ります。
	/// </summary>
	protected override void OnNavigatedTo(NavigationEventArgs e) {
		if (e.Parameter is HighlightSettingsPageViewModel vm) {
			this.ViewModel = vm;
		} else {
			throw new InvalidOperationException("ViewModel is not passed.");
		}
		base.OnNavigatedTo(e);
	}
}
