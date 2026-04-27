using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SabreTools.Serialization.UI.Desktop.Services;
using SabreTools.Serialization.UI.Desktop.ViewModels;

namespace SabreTools.Serialization.UI.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new SerializationWorkbenchService());
        }

        private async void BrowseFileButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
                return;

            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Select a file to inspect",
            });

            var file = files.Count > 0 ? files[0] : null;
            if (file is null)
                return;

            var localPath = file.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(localPath))
                viewModel.SetFilePath(localPath);
        }

        private async void BrowseFolderButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
                return;

            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Select an extraction folder",
            });

            var folder = folders.Count > 0 ? folders[0] : null;
            if (folder is null)
                return;

            var localPath = folder.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(localPath))
                viewModel.OutputDirectory = localPath;
        }

        private async void InspectButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                await viewModel.InspectSelectedFileAsync();
        }

        private async void ExtractButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                await viewModel.ExtractCurrentFileAsync();
        }

        private void ShowJsonTreeTab_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.SelectWorkspaceTab("JsonTree");
        }

        private void ShowPrintedInfoTab_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.SelectWorkspaceTab("PrintedInfo");
        }

        private void ShowRawJsonTab_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.SelectWorkspaceTab("RawJson");
        }
    }
}
