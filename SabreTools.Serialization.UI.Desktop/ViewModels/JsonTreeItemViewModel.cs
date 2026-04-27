using System.Collections.ObjectModel;
using SabreTools.Serialization.UI.Desktop.Infrastructure;

namespace SabreTools.Serialization.UI.Desktop.ViewModels
{
    public sealed class JsonTreeItemViewModel : ObservableObject
    {
        private bool _isExpanded;

        public JsonTreeItemViewModel(string name, string summary, bool isExpanded = false)
        {
            Name = name;
            Summary = summary;
            _isExpanded = isExpanded;
        }

        public string Name { get; }

        public string Summary { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetField(ref _isExpanded, value);
        }

        public ObservableCollection<JsonTreeItemViewModel> Children { get; } = new();
    }
}
