using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using SabreTools.Serialization.UI.Desktop.ViewModels;

namespace SabreTools.Serialization.UI.Desktop.Services
{
    public static class JsonTreeBuilder
    {
        public static ObservableCollection<JsonTreeItemViewModel> Build(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new ObservableCollection<JsonTreeItemViewModel>();

            JsonNode? root = JsonNode.Parse(json);
            if (root is null)
                return new ObservableCollection<JsonTreeItemViewModel>();

            return new ObservableCollection<JsonTreeItemViewModel>
            {
                CreateNode("root", root),
            };
        }

        private static JsonTreeItemViewModel CreateNode(string name, JsonNode? node)
        {
            if (node is JsonObject obj)
            {
                var item = new JsonTreeItemViewModel(name, $"{{{obj.Count} item(s)}}");
                foreach (KeyValuePair<string, JsonNode?> pair in obj)
                {
                    item.Children.Add(CreateNode(pair.Key, pair.Value));
                }

                return item;
            }

            if (node is JsonArray arr)
            {
                var item = new JsonTreeItemViewModel(name, $"[{arr.Count} item(s)]");
                for (int i = 0; i < arr.Count; i++)
                {
                    item.Children.Add(CreateNode($"[{i}]", arr[i]));
                }

                return item;
            }

            string summary = node is null
                ? "null"
                : JsonSerializer.Serialize(node);

            return new JsonTreeItemViewModel(name, summary);
        }
    }
}
