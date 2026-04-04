using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RemoteLogViewer.Composition.Stores.Settings;
using RemoteLogViewer.Core.Services;
using RemoteLogViewer.Core.Services.Viewer;
using RemoteLogViewer.Core.Stores.Settings;
using Xunit;

namespace RemoteLogViewer.Core.Tests.Services.Viewer;

public class HighlightServiceTests {
	[Fact]
	public void CreateStyledLine_ShouldHighlightCorrectly() {
		var serviceProvider = CreateServiceProvider();
		var settingsStore = serviceProvider.GetRequiredService<SettingsStoreModel>();
		var highlightService = serviceProvider.GetRequiredService<HighlightService>();

		var highlightSettings = settingsStore.SettingsModel.HighlightSettings;
		var rule1 = highlightSettings.AddRule();
		var cond1 = rule1.AddCondition();
		cond1.Pattern.Value = "test";
		cond1.PatternType.Value = HighlightPatternType.Exact;
		cond1.HighlightOnlyMatch.Value = true;

		highlightService.CreateCss(".wrapper");

		var result = highlightService.CreateStyledLine("this is a test line");

		Assert.Contains("<span class=\"c0\">test</span>", result);
	}

	[Fact]
	public void CreateStyledLine_OverlappingStyles_ShouldHandleCorrectly() {
		var serviceProvider = CreateServiceProvider();
		var settingsStore = serviceProvider.GetRequiredService<SettingsStoreModel>();
		var highlightService = serviceProvider.GetRequiredService<HighlightService>();

		var highlightSettings = settingsStore.SettingsModel.HighlightSettings;

		// Priority 0 (Lower number, higher priority)
		var rule0 = highlightSettings.AddRule();
		var cond0 = rule0.AddCondition();
		cond0.Pattern.Value = "abcde";
		cond0.PatternType.Value = HighlightPatternType.Exact;
		cond0.HighlightOnlyMatch.Value = true;

		// Priority 1
		var rule1 = highlightSettings.AddRule();
		var cond1 = rule1.AddCondition();
		cond1.Pattern.Value = "bcd";
		cond1.PatternType.Value = HighlightPatternType.Exact;
		cond1.HighlightOnlyMatch.Value = true;

		highlightService.CreateCss(".wrapper");

		var result = highlightService.CreateStyledLine("abcde");

		// Expected behavior depends on implementation details of nesting,
		// but both classes should be present in some form.
		Assert.Contains("c0", result);
		Assert.Contains("c1", result);
	}

	private static IServiceProvider CreateServiceProvider() {
		var services = new ServiceCollection();
		services.AddSingleton(Mock.Of<ILogger<SettingsStoreModel>>());
		services.AddSingleton(Mock.Of<ILogger<WorkspaceService>>());
		services.AddSingleton<WorkspaceService>();
		services.AddSingleton<SettingsStoreModel>();
		services.AddTransient<HighlightService>();
		services.AddScoped<HighlightConditionModel>(sp => new HighlightConditionModel(sp));
		services.AddSingleton<SettingsModel>();
		services.AddSingleton<HighlightSettingsModel>();
		services.AddSingleton<TextViewerSettingsModel>();
		services.AddSingleton<AdvancedSettingsModel>();
		services.AddScoped<HighlightRuleModel>();
		return services.BuildServiceProvider();
	}
}
