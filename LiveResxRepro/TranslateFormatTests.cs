using System.Globalization;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResxRepro;

public sealed class TranslateFormatTests
{
    private static (TControl View, GreetingViewModel Vm, Window Window) Show<TControl>()
        where TControl : Control, new()
    {
        // DynamicTranslation.Text throws if no culture has been set yet.
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

        var vm = new GreetingViewModel { Name = "Ana" };
        var view = new TControl { DataContext = vm };
        var window = new Window { Content = view };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return (view, vm, window);
    }

    private static void SwitchLocale(string culture)
    {
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo(culture));
        Dispatcher.UIThread.RunJobs();
    }

    private static void AssertNoBindingWarnings(BindingLogCapture log) =>
        Assert.True(log.Messages.Count == 0,
            "Binding warnings:" + Environment.NewLine + string.Join(Environment.NewLine, log.Messages));

    [AvaloniaFact]
    public void Translate_SwitchesLanguage() // control
    {
        var (view, _, _) = Show<GreetingView>();
        var text = view.FindControl<TextBlock>("ViaTranslate")!;
        Assert.Equal("Hello, {0}!", text.Text);

        SwitchLocale("de");

        Assert.Equal("Hallo, {0}!", text.Text);
    }

    [AvaloniaFact]
    public void TranslateFormat_SwitchesLanguage()
    {
        var (view, _, _) = Show<GreetingView>();
        var text = view.FindControl<TextBlock>("ViaTranslateFormat")!;
        Assert.Equal("Hello, Ana!", text.Text);

        SwitchLocale("de");

        Assert.Equal("Hallo, Ana!", text.Text);
    }

    [AvaloniaFact]
    public void TranslateFormat_LogsNoBindingWarnings()
    {
        using var log = new BindingLogCapture();
        Show<GreetingView>();
        SwitchLocale("de");

        AssertNoBindingWarnings(log);
    }

    [AvaloniaFact]
    public void TranslateFormat_RefreshesOnlyWhenAnArgumentChanges()
    {
        var (view, vm, _) = Show<GreetingView>();
        var text = view.FindControl<TextBlock>("ViaTranslateFormat")!;

        SwitchLocale("de");
        vm.Name = "Ion";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("Hallo, Ion!", text.Text);
    }

    [AvaloniaFact]
    public void Patched_SwitchesLanguage_AndLogsNoBindingWarnings()
    {
        using var log = new BindingLogCapture();
        var (view, _, _) = Show<PatchedGreetingView>();
        var text = view.FindControl<TextBlock>("ViaPatchedTranslateFormat")!;
        Assert.Equal("Hello, Ana!", text.Text);

        SwitchLocale("de");

        Assert.Equal("Hallo, Ana!", text.Text);
        AssertNoBindingWarnings(log);
    }
}
