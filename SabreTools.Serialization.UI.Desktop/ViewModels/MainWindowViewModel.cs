using System.Collections.ObjectModel;
using SabreTools.Serialization.UI.Desktop.Infrastructure;
using SabreTools.Serialization.UI.Desktop.Models;
using SabreTools.Serialization.UI.Desktop.Services;

namespace SabreTools.Serialization.UI.Desktop.ViewModels
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private enum WorkspaceTab
        {
            JsonTree,
            PrintedInfo,
            RawJson,
        }

        private readonly SerializationWorkbenchService _service;

        private string _filePath = string.Empty;
        private string _outputDirectory = string.Empty;
        private string _wrapperType = "None";
        private string _description = "No file inspected yet.";
        private string _jsonOutput = string.Empty;
        private string _printableOutput = string.Empty;
        private string _errorMessage = "None";
        private string _statusMessage = "Ready to inspect a file.";
        private bool _isBusy;
        private WorkspaceTab _selectedWorkspaceTab;
        private InspectionResult? _currentInspection;

        public MainWindowViewModel(SerializationWorkbenchService service)
        {
            _service = service;
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (SetField(ref _filePath, value))
                {
                    OnPropertyChanged(nameof(CanInspect));
                    OnPropertyChanged(nameof(FileNameDisplay));
                }
            }
        }

        public string OutputDirectory
        {
            get => _outputDirectory;
            set
            {
                if (SetField(ref _outputDirectory, value))
                {
                    OnPropertyChanged(nameof(CanExtract));
                    OnPropertyChanged(nameof(OutputDirectoryDisplay));
                }
            }
        }

        public string WrapperType
        {
            get => _wrapperType;
            private set => SetField(ref _wrapperType, value);
        }

        public string Description
        {
            get => _description;
            private set => SetField(ref _description, value);
        }

        public string JsonOutput
        {
            get => _jsonOutput;
            private set => SetField(ref _jsonOutput, value);
        }

        public string PrintableOutput
        {
            get => _printableOutput;
            private set => SetField(ref _printableOutput, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetField(ref _errorMessage, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetField(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetField(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(BusyState));
                    OnPropertyChanged(nameof(CanInspect));
                    OnPropertyChanged(nameof(CanExtract));
                }
            }
        }

        public string BusyState => IsBusy ? "Working..." : "Idle";

        public string PrintableState => _currentInspection?.CanPrint == true ? "Yes" : "No";

        public string ExtractableState => _currentInspection?.CanExtract == true ? "Yes" : "No";

        public bool CanInspect => !IsBusy && !string.IsNullOrWhiteSpace(FilePath);

        public bool CanExtract
            => !IsBusy
            && _currentInspection?.CanExtract == true
            && !string.IsNullOrWhiteSpace(OutputDirectory)
            && !string.IsNullOrWhiteSpace(FilePath);

        public string FileNameDisplay
            => string.IsNullOrWhiteSpace(FilePath)
                ? "No file selected."
                : Path.GetFileName(FilePath);

        public string OutputDirectoryDisplay
            => string.IsNullOrWhiteSpace(OutputDirectory)
                ? "No output folder selected."
                : OutputDirectory;

        public bool IsJsonTreeTabSelected => _selectedWorkspaceTab == WorkspaceTab.JsonTree;

        public bool IsPrintedInfoTabSelected => _selectedWorkspaceTab == WorkspaceTab.PrintedInfo;

        public bool IsRawJsonTabSelected => _selectedWorkspaceTab == WorkspaceTab.RawJson;

        public ObservableCollection<JsonTreeItemViewModel> JsonTreeItems { get; private set; } = new();

        public void SetFilePath(string filePath)
        {
            FilePath = filePath;

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                string filename = Path.GetFileNameWithoutExtension(filePath);
                OutputDirectory = Path.Combine(directory, $"{filename}-extract");
            }
        }

        public async Task InspectSelectedFileAsync()
        {
            if (!CanInspect)
                return;

            IsBusy = true;
            StatusMessage = "Inspecting file through the SabreTools wrapper factory...";
            ErrorMessage = "None";

            try
            {
                InspectionResult result = await _service.InspectAsync(FilePath);
                _currentInspection = result;

                WrapperType = result.WrapperType;
                Description = string.IsNullOrWhiteSpace(result.Description)
                    ? "No description available."
                    : result.Description;
                JsonOutput = result.JsonOutput ?? string.Empty;
                PrintableOutput = result.PrintableOutput ?? string.Empty;
                ErrorMessage = result.ErrorMessage ?? "None";
                JsonTreeItems = JsonTreeBuilder.Build(result.JsonOutput);

                OnPropertyChanged(nameof(JsonTreeItems));
                OnPropertyChanged(nameof(PrintableState));
                OnPropertyChanged(nameof(ExtractableState));
                OnPropertyChanged(nameof(CanExtract));

                StatusMessage = result.ErrorMessage is null
                    ? "Inspection complete."
                    : $"Inspection finished with an issue: {result.ErrorMessage}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExtractCurrentFileAsync()
        {
            if (!CanExtract)
                return;

            IsBusy = true;
            StatusMessage = "Extracting all supported contents...";

            try
            {
                ExtractionResult result = await _service.ExtractAsync(FilePath, OutputDirectory, includeDebug: false);
                StatusMessage = result.Message;
                ErrorMessage = result.Succeeded ? "None" : result.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void SelectWorkspaceTab(string tabKey)
        {
            WorkspaceTab nextTab = tabKey switch
            {
                "PrintedInfo" => WorkspaceTab.PrintedInfo,
                "RawJson" => WorkspaceTab.RawJson,
                _ => WorkspaceTab.JsonTree,
            };

            if (_selectedWorkspaceTab == nextTab)
                return;

            _selectedWorkspaceTab = nextTab;
            OnPropertyChanged(nameof(IsJsonTreeTabSelected));
            OnPropertyChanged(nameof(IsPrintedInfoTabSelected));
            OnPropertyChanged(nameof(IsRawJsonTabSelected));
        }
    }
}
