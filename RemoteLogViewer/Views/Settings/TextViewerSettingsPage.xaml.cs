using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using RemoteLogViewer.ViewModels.Settings;


namespace RemoteLogViewer.Views.Settings;

public sealed partial class TextViewerSettingsPage : Page {
	public TextViewerSettingsPageViewModel? ViewModel {
		get;
		private set;
	}

	public TextViewerSettingsPage() {
		this.InitializeComponent();
	}
	/// <summary>
	/// ナビゲート時に ViewModel を受け取ります。
	/// </summary>
	protected override void OnNavigatedTo(NavigationEventArgs e) {
		if (e.Parameter is not TextViewerSettingsPageViewModel vm) {
			throw new InvalidOperationException("ViewModel is not passed.");
		}
		this.ViewModel = vm;
		base.OnNavigatedTo(e);
	}
}
