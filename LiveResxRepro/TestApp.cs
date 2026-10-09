using Avalonia;
using Avalonia.Headless;
using LiveResxRepro;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(TestApp))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace LiveResxRepro;

public sealed class TestApp : Application
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
