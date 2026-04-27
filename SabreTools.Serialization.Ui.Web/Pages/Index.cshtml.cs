using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SabreTools.Serialization.Wrappers;

namespace SabreTools.Serialization.Ui.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public IFormFile? Upload { get; set; }

    public AnalysisResult? Result { get; private set; }

    public string[] SampleCapabilities { get; } = new[]
    {
        "Executables and embedded installers",
        "Archives including ZIP, gzip, TAR, 7z, and CAB",
        "Disc and cartridge formats like ISO, NES, FDS, and Atari",
        "Printable JSON output via SabreTools wrappers",
    };

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Upload is null || Upload.Length == 0)
        {
            ModelState.AddModelError(nameof(Upload), "Choose a file to inspect.");
            return Page();
        }

        try
        {
            await using var input = Upload.OpenReadStream();
            await using var workingCopy = new MemoryStream();
            await input.CopyToAsync(workingCopy);
            workingCopy.Position = 0;

            var magic = new byte[Math.Min(16, (int)workingCopy.Length)];
            _ = await workingCopy.ReadAsync(magic, 0, magic.Length);
            workingCopy.Position = 0;

            var extension = Path.GetExtension(Upload.FileName).TrimStart('.');
            var wrapperType = WrapperFactory.GetFileType(magic, extension);
            var wrapper = WrapperFactory.CreateWrapper(wrapperType, workingCopy);

            if (wrapper is null)
            {
                Result = new AnalysisResult(
                    Upload.FileName,
                    wrapperType.ToString(),
                    false,
                    "SabreTools could not create a wrapper for this file.",
                    null,
                    null);
                return Page();
            }

            if (wrapper is not IPrintable printable)
            {
                Result = new AnalysisResult(
                    Upload.FileName,
                    wrapperType.ToString(),
                    false,
                    "The file was detected, but this wrapper does not expose printable output yet.",
                    null,
                    null);
                return Page();
            }

            var builder = new StringBuilder();
            printable.PrintInformation(builder);

            Result = new AnalysisResult(
                Upload.FileName,
                wrapperType.ToString(),
                true,
                null,
                printable.ExportJSON(),
                builder.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inspect uploaded file {FileName}", Upload?.FileName);
            Result = new AnalysisResult(
                Upload?.FileName ?? "Unknown file",
                "Unknown",
                false,
                ex.Message,
                null,
                null);
        }

        return Page();
    }
}

public sealed record AnalysisResult(
    string FileName,
    string WrapperType,
    bool IsPrintable,
    string? ErrorMessage,
    string? JsonOutput,
    string? TextOutput);
