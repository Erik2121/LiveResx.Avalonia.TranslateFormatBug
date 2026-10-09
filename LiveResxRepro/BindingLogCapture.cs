using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Logging;

namespace LiveResxRepro;

/// <summary>
/// Captures Avalonia binding warnings/errors while in scope, then restores the previous sink.
/// </summary>
public sealed partial class BindingLogCapture : ILogSink, IDisposable
{
    private readonly ILogSink? _previous;
    private readonly List<string> _messages = [];

    public BindingLogCapture()
    {
        _previous = Logger.Sink;
        Logger.Sink = this;
    }

    public IReadOnlyList<string> Messages => _messages;

    public bool IsEnabled(LogEventLevel level, string area) =>
        (area == LogArea.Binding && level >= LogEventLevel.Warning) || (_previous?.IsEnabled(level, area) ?? false);

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate) =>
        Log(level, area, source, messageTemplate, []);

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
    {
        if (area == LogArea.Binding && level >= LogEventLevel.Warning)
        {
            // Same format as Avalonia's LogToTrace() output: "[Area]message (Source)"
            var i = 0;
            var text = Placeholder().Replace(messageTemplate, m => i < propertyValues.Length ? $"'{propertyValues[i++]}'" : m.Value);
            _messages.Add($"[{area}]{text}{FormatSource(source)}");
        }

        if (_previous?.IsEnabled(level, area) == true)
            _previous.Log(level, area, source, messageTemplate, propertyValues);
    }

    public void Dispose() => Logger.Sink = _previous;

    private static string FormatSource(object? source) => source switch
    {
        null => "",
        StyledElement { Name: { } name } => $" ({source.GetType().Name} #{name})",
        _ => $" ({source.GetType().Name} #{source.GetHashCode()})",
    };

    [GeneratedRegex(@"\{[^}]+\}")]
    private static partial Regex Placeholder();
}
