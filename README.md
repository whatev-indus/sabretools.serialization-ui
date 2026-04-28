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

- `SabreTools.Serialization.UI.Desktop`: primary Avalonia desktop app

## Run it

```bash
env DOTNET_CLI_HOME=/tmp/dotnet-home dotnet restore
env DOTNET_CLI_HOME=/tmp/dotnet-home dotnet run --project SabreTools.Serialization.UI.Desktop
```

## Behavior Notes

- Printable wrappers populate the JSON tree and printed info tabs
- Non-printable wrappers still show detection details and extract support state
- Extraction uses the wrapper's built-in logic directly, mirroring `ExtractionTool`


<img width="1472" height="1040" alt="Screenshot 2026-04-27 at 9 21 36 PM" src="https://github.com/user-attachments/assets/ab32cf4d-e68e-4d12-aa39-42a13dd107bf" />
