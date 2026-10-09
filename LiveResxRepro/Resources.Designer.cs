#nullable enable
using System.Globalization;
using System.Resources;

namespace LiveResxRepro;

// Hand-written equivalent of a ResXFileCodeGenerator designer class.
// LiveResx.Avalonia detects it via the static ResourceManager/Culture properties
// and treats the static string properties as resource keys.
public static class Resources
{
    private static ResourceManager? _resourceManager;

    public static ResourceManager ResourceManager =>
        _resourceManager ??= new ResourceManager("LiveResxRepro.Resources", typeof(Resources).Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string Greeting => ResourceManager.GetString(nameof(Greeting), Culture)!;
}
