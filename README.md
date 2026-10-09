# LiveResx.Avalonia – `TranslateFormat` repro

Minimal, standalone reproduction of a bug in the source-generated `TranslateFormatExtension` of
[LiveResx.Avalonia](https://github.com/AndreiLacatos/LiveResx.Avalonia) **2.0.0** (Avalonia 12.1.0, .NET 10).

The generated `ProvideValue` adds `new ReflectionBinding("Text")` to the `MultiBinding` **without a `Source`**, so
`Text` is resolved against the target's `DataContext` instead of the `Template` (`DynamicTranslation`). As a result:

1. a binding warning is logged whenever the DataContext has no `Text` property, and
2. after `DynamicLocalization.Instance.SwitchLocale(...)` the text stays in the old language until one of the bound
   arguments changes.

The one-line fix is `new ReflectionBinding("Text") { Source = Template }`.
[`PatchedTranslateFormatExtension.cs`](LiveResxRepro/PatchedTranslateFormatExtension.cs) is a verbatim copy of the
generated file with only that change (plus a rename), and its tests pass.

## Run

```bash
dotnet test LiveResxRepro
```

## Expected results on 2.0.0

| Test | Result |
|---|---|
| `Translate_SwitchesLanguage` (control: plain `Translate`) | pass |
| `TranslateFormat_SwitchesLanguage` | **fail**: stays `Hello, Ana!` |
| `TranslateFormat_LogsNoBindingWarnings` | **fail**: `Could not find a matching property accessor for 'Text' on 'LiveResxRepro.GreetingViewModel'` |
| `TranslateFormat_RefreshesOnlyWhenAnArgumentChanges` | pass (text updates once `Name` changes) |
| `Patched_SwitchesLanguage_AndLogsNoBindingWarnings` | pass |

## Layout

- `GreetingViewModel.cs`: ViewModel with only a `Name` property (deliberately no `Text`).
- `GreetingView.axaml`: `TranslateFormat` (bug) and `Translate` (control) side by side.
- `PatchedGreetingView.axaml` + `PatchedTranslateFormatExtension.cs`: the same view using the fixed copy.
- `Resources.resx` / `Resources.de.resx` / `Resources.Designer.cs`: `Greeting = "Hello, {0}!"` / `"Hallo, {0}!"`.
- `BindingLogCapture.cs`: temporary `ILogSink` collecting `Binding` warnings.
