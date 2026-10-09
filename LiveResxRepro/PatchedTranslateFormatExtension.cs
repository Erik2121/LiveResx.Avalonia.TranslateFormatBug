// Verbatim copy of the LiveResx.Avalonia 2.0.0 generated LiveResx.Avalonia.TranslateFormatExtension.g.cs,
// with exactly two changes:
//   1. renamed to PatchedTranslateFormatExtension in namespace LiveResxRepro (to avoid clashing with the generated one)
//   2. THE FIX: the "Text" binding gets Source = Template, like TranslateExtension does.
#nullable enable
#pragma warning disable

namespace LiveResxRepro
{
    /// <summary>
    /// Provides a markup extension that returns a <see cref="global::Avalonia.Data.MultiBinding"/>
    /// combining a format-template <see cref="DynamicTranslation"/> with additional data-bound
    /// arguments, enabling localized composite strings that update automatically when the
    /// application culture changes.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("LiveResx.Avalonia.SourceGenerators", "2.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class PatchedTranslateFormatExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="DynamicTranslation"/> that provides the format template string.
        /// </summary>
        public global::LiveResx.Avalonia.DynamicTranslation Template { get; set; }

        /// <summary>
        /// Gets the collection of bindings that supply the format arguments.
        /// </summary>
        [global::Avalonia.Metadata.Content]
        public global::System.Collections.Generic.IList<global::Avalonia.Data.BindingBase> Bindings { get; } =
            new global::System.Collections.Generic.List<global::Avalonia.Data.BindingBase>();

        /// <summary>
        /// Provides the value for the markup extension, returning a <see cref="global::Avalonia.Data.MultiBinding"/>
        /// that applies <see cref="string.Format(string, object[])"/> using the template from
        /// <see cref="Template"/> and the values from <see cref="Bindings"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider for the markup extension.</param>
        /// <returns>A <see cref="global::Avalonia.Data.MultiBinding"/> that produces the formatted string.</returns>
        public object ProvideValue(global::System.IServiceProvider serviceProvider)
        {
            var mb = new global::Avalonia.Data.MultiBinding();

            foreach (var binding in Bindings)
            {
                mb.Bindings.Add(binding);
            }

            mb.Bindings.Add(new global::Avalonia.Data.ReflectionBinding("Text") { Source = Template }); // <-- the fix
            mb.Converter = new TranslationTemplateConverter(Template);
            return mb;
        }

        private sealed class TranslationTemplateConverter : global::Avalonia.Data.Converters.IMultiValueConverter
        {
            private readonly global::LiveResx.Avalonia.DynamicTranslation _key;

            public TranslationTemplateConverter(global::LiveResx.Avalonia.DynamicTranslation key)
            {
                _key = key;
            }

            public object Convert(
                global::System.Collections.Generic.IList<object> values,
                global::System.Type targetType,
                object parameter,
                global::System.Globalization.CultureInfo culture)
            {
                object[] args;

                if (values.Count <= 1)
                {
                    args = global::System.Array.Empty<object>();
                }
                else
                {
                    args = new object[values.Count - 1];

                    for (var i = 0; i < args.Length; i++)
                    {
                        args[i] = values[i];
                    }
                }
                var template = _key.Text;
                return string.Format(template, args);
            }
        }
    }
}
