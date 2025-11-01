using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RemoteLogViewer.Stores.Settings.Model;

namespace RemoteLogViewer.ViewModels.Settings.Highlight;

[AddSingleton]
public class HighlightSettingsPageViewModel : SettingsPageViewModel<HighlightSettingsPageViewModel> {
	/// <summary>
	/// ルールリスト。
	/// </summary>
	public NotifyCollectionChangedSynchronizedViewList<HighlightRuleViewModel> Rules {
		get;
	}

	public BindableReactiveProperty<HighlightRuleViewModel?> SelectedRule {
		get;
	} = new();

	public ReactiveCommand AddRuleCommand { get; } = new();
	public ReactiveCommand<HighlightRuleViewModel> RemoveRuleCommand { get; } = new();

	public HighlightPatternType[] PatternTypes {
		get;
	} = Enum.GetValues<HighlightPatternType>();

	public HighlightSettingsPageViewModel(HighlightSettingsModel model, IServiceProvider service, ILogger<HighlightSettingsPageViewModel> logger) : base("Highlight", logger) {
		// View生成
		var view = model.Rules.CreateView(x => x.ScopedService.GetRequiredService<HighlightRuleViewModel>()).AddTo(this.CompositeDisposable);
		this.Rules = view.ToNotifyCollectionChanged().AddTo(this.CompositeDisposable);
		this.SelectedRule.Value = this.Rules.FirstOrDefault();
		this.AddRuleCommand.Subscribe(_ => {
			var rule = model.AddRule();
			this.SelectedRule.Value = rule.ScopedService.GetRequiredService<HighlightRuleViewModel>();
		}).AddTo(this.CompositeDisposable);

		this.RemoveRuleCommand.Subscribe(rule => {
			if (rule == null) {
				return;
			}
			model.removeRule(rule.Model);
			if (this.SelectedRule.Value == null) {
				this.SelectedRule.Value = this.Rules.LastOrDefault();
			}
		}).AddTo(this.CompositeDisposable);
	}
}
