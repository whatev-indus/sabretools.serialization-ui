using Avalonia.Controls;
using Avalonia.Input;
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
            AddHandler(DragDrop.DragOverEvent, MainWindow_OnDragOver);
            AddHandler(DragDrop.DropEvent, MainWindow_OnDrop);
        }

        private void MainWindow_OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = HasDroppedFile(e) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private async void MainWindow_OnDrop(object? sender, DragEventArgs e)
        {
            if (DataContext is not MainWindowViewModel viewModel)
                return;

            string? localPath = await GetFirstDroppedLocalPathAsync(e);
            if (string.IsNullOrWhiteSpace(localPath))
                return;

            viewModel.SetFilePath(localPath);
            await viewModel.InspectSelectedFileAsync();
            e.Handled = true;
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

        private static bool HasDroppedFile(DragEventArgs e)
            => e.Data.Contains(DataFormats.Files);

        private static Task<string?> GetFirstDroppedLocalPathAsync(DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files))
                return Task.FromResult<string?>(null);

            IEnumerable<IStorageItem>? items = e.Data.Get(DataFormats.Files) as IEnumerable<IStorageItem>;
            IStorageFile? file = items?.OfType<IStorageFile>().FirstOrDefault();
            return Task.FromResult(file?.TryGetLocalPath());
        }
    }
}
