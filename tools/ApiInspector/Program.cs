using System.Reflection;

var assemblyPath = "/tmp/dotnet-home/.nuget/packages/sabretools.serialization/2.3.0/lib/net7.0/SabreTools.Serialization.dll";
var assembly = Assembly.LoadFrom(assemblyPath);

var targetTypes = new[]
{
    "SabreTools.Serialization.Wrappers.WrapperFactory",
    "SabreTools.Serialization.Wrappers.IWrapper",
    "SabreTools.Serialization.Wrappers.IPrintable",
    "SabreTools.Serialization.Wrappers.IExtractable",
    "SabreTools.Serialization.Readers.NewExecutable",
};

foreach (var typeName in targetTypes)
{
    var type = assembly.GetType(typeName);
    if (type is null)
    {
        Console.WriteLine($"{typeName}: not found");
        continue;
    }

    Console.WriteLine(type.FullName);
    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .OrderBy(m => m.Name))
    {
        var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
        Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({parameters})");
    }
}
