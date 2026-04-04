using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RemoteLogViewer.Composition.Stores.Settings;
using RemoteLogViewer.Core.Services;
using RemoteLogViewer.Core.Services.Viewer;
using RemoteLogViewer.Core.Stores.Settings;
using Xunit;
using Xunit.Abstractions;

namespace RemoteLogViewer.Core.Tests.Services.Viewer;

public class HighlightServiceBenchmark {
	private readonly ITestOutputHelper _output;

	public HighlightServiceBenchmark(ITestOutputHelper output) {
		this._output = output;
	}

	[Fact]
	public void BenchmarkCreateStyledLine() {
		var serviceProvider = CreateServiceProvider();
		var settingsStore = serviceProvider.GetRequiredService<SettingsStoreModel>();
		var highlightService = serviceProvider.GetRequiredService<HighlightService>();
		var grepCondition = serviceProvider.GetRequiredService<HighlightConditionModel>();

		// Setup many rules
		var highlightSettings = settingsStore.SettingsModel.HighlightSettings;
		for (int i = 0; i < 100; i++) {
			var rule = highlightSettings.AddRule();
			var condition = rule.AddCondition();
			condition.Pattern.Value = $"test{i}";
			condition.PatternType.Value = HighlightPatternType.Exact;
			condition.HighlightOnlyMatch.Value = true;
		}

		// Initialize CSS to populate _ruleWithClassName
		highlightService.CreateCss(".wrapper");

		var content = string.Join(" ", Enumerable.Range(0, 1000).Select(i => $"test{i % 100}"));

		// Warmup
		for (int i = 0; i < 10; i++) {
			highlightService.CreateStyledLine(content);
		}

		var sw = Stopwatch.StartNew();
		int iterations = 100;
		for (int i = 0; i < iterations; i++) {
			highlightService.CreateStyledLine(content);
		}
		sw.Stop();

		this._output.WriteLine($"Total time for {iterations} iterations: {sw.ElapsedMilliseconds}ms");
		this._output.WriteLine($"Average time per iteration: {sw.Elapsed.TotalMilliseconds / iterations}ms");
	}

	private static IServiceProvider CreateServiceProvider() {
		var services = new ServiceCollection();
		services.AddSingleton(Mock.Of<ILogger<SettingsStoreModel>>());
		services.AddSingleton(Mock.Of<ILogger<WorkspaceService>>());
		services.AddSingleton(Mock.Of<ILogger<SettingsModel>>()); // Not really needed if we use implementation
		services.AddSingleton<WorkspaceService>();
		services.AddSingleton<SettingsStoreModel>();
		services.AddTransient<HighlightService>();

		// Mock HighlightConditionModel (GREP)
		services.AddScoped<HighlightConditionModel>(sp => new HighlightConditionModel(sp));

		// Setup for SettingsStoreModel
		services.AddSingleton<SettingsModel>();
		services.AddSingleton<HighlightSettingsModel>();
		services.AddSingleton<TextViewerSettingsModel>();
		services.AddSingleton<AdvancedSettingsModel>();
		services.AddScoped<HighlightRuleModel>();

		return services.BuildServiceProvider();
	}
}
