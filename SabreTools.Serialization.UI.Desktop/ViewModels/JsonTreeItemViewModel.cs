using System.Collections.ObjectModel;

namespace SabreTools.Serialization.UI.Desktop.ViewModels
{
    public sealed class JsonTreeItemViewModel
    {
        public JsonTreeItemViewModel(string name, string summary)
        {
            Name = name;
            Summary = summary;
        }

        public string Name { get; }

        public string Summary { get; }

        public ObservableCollection<JsonTreeItemViewModel> Children { get; } = new();
    }
}
