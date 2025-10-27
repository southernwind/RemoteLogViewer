using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using RemoteLogViewer.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RemoteLogViewer.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
[AddSingleton]
public sealed partial class MainWindow : Window {
	public MainWindow(MainWindowViewModel mainWindowViewModel) {
		this.InitializeComponent();
		this.ViewModel = mainWindowViewModel;
	}

	public MainWindowViewModel ViewModel {
		get;
	}

	private void TabView_TabCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e) {
		if (e.Item is LogViewerViewModel vm) {
			this.ViewModel.CloseTabCommand.Execute(vm);
		}
	}
}
