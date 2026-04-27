# SabreTools.Serialization UI

An Avalonia desktop GUI for [`SabreTools.Serialization`](https://github.com/SabreTools/SabreTools.Serialization) that combines the reference behavior of `InfoPrint` and `ExtractionTool`.

## What it does

- Open a file and inspect it through the upstream wrapper factory
- Export the same JSON emitted by `IPrintable.ExportJSON()`
- Render that JSON as a collapsible tree for easier browsing
- Show the plain-text `PrintInformation(...)` output from the wrapper
- Pick an output folder and run `IExtractable.Extract(...)` when supported

The application intentionally keeps its own logic thin. Detection, printing, and extraction all come from the `SabreTools.Serialization` library so future wrapper improvements should flow into the GUI naturally.

## Projects

- `SabreTools.Serialization.Ui.Desktop`: primary Avalonia desktop app
- `SabreTools.Serialization.Ui.Web`: earlier Razor Pages starter kept in the repo, but not the main target for this GUI effort

## Run it

```bash
env DOTNET_CLI_HOME=/tmp/dotnet-home dotnet restore
env DOTNET_CLI_HOME=/tmp/dotnet-home dotnet run --project SabreTools.Serialization.Ui.Desktop
```

## Behavior Notes

- Printable wrappers populate the JSON tree and printed info tabs
- Non-printable wrappers still show detection details and extract support state
- Extraction uses the wrapper's built-in logic directly, mirroring `ExtractionTool`
