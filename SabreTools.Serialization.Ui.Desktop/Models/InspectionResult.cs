namespace SabreTools.Serialization.Ui.Desktop.Models
{
    public sealed record InspectionResult(
        string FilePath,
        string WrapperType,
        string Description,
        bool CanPrint,
        bool CanExtract,
        string? JsonOutput,
        string? PrintableOutput,
        string? ErrorMessage);
}
