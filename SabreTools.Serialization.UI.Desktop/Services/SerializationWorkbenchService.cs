using System.Text;
using SabreTools.Serialization.UI.Desktop.Models;
using SabreTools.Serialization.Wrappers;

namespace SabreTools.Serialization.UI.Desktop.Services
{
    public sealed class SerializationWorkbenchService
    {
        public Task<InspectionResult> InspectAsync(string filePath)
        {
            return Task.Run(() => Inspect(filePath));
        }

        public Task<ExtractionResult> ExtractAsync(string filePath, string outputDirectory, bool includeDebug)
        {
            return Task.Run(() => Extract(filePath, outputDirectory, includeDebug));
        }

        private static InspectionResult Inspect(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new InspectionResult(
                    filePath,
                    "Unknown",
                    string.Empty,
                    false,
                    false,
                    null,
                    null,
                    "The selected file does not exist.");
            }

            try
            {
                using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                byte[] magic = ReadMagic(stream);
                stream.Position = 0;

                string extension = Path.GetExtension(filePath).TrimStart('.');
                WrapperType wrapperType = WrapperFactory.GetFileType(magic, extension);
                IWrapper? wrapper = WrapperFactory.CreateWrapper(wrapperType, stream);

                if (wrapper is null)
                {
                    return new InspectionResult(
                        filePath,
                        wrapperType.ToString(),
                        string.Empty,
                        false,
                        false,
                        null,
                        null,
                        "Either the wrapper is unsupported or parsing failed.");
                }

                string? jsonOutput = null;
                string? printableOutput = null;
                bool canPrint = wrapper is IPrintable;
                bool canExtract = wrapper is IExtractable;

                if (wrapper is IPrintable printable)
                {
                    jsonOutput = printable.ExportJSON();

                    var builder = new StringBuilder();
                    printable.PrintInformation(builder);
                    printableOutput = builder.ToString();
                }

                return new InspectionResult(
                    filePath,
                    wrapperType.ToString(),
                    wrapper.Description(),
                    canPrint,
                    canExtract,
                    jsonOutput,
                    printableOutput,
                    null);
            }
            catch (Exception ex)
            {
                return new InspectionResult(
                    filePath,
                    "Unknown",
                    string.Empty,
                    false,
                    false,
                    null,
                    null,
                    ex.Message);
            }
        }

        private static ExtractionResult Extract(string filePath, string outputDirectory, bool includeDebug)
        {
            if (!File.Exists(filePath))
                return new ExtractionResult(false, "The selected file does not exist.");

            if (string.IsNullOrWhiteSpace(outputDirectory))
                return new ExtractionResult(false, "Choose an output directory before extracting.");

            try
            {
                outputDirectory = Path.GetFullPath(outputDirectory);
                Directory.CreateDirectory(outputDirectory);

                using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                byte[] magic = ReadMagic(stream);
                stream.Position = 0;

                string extension = Path.GetExtension(filePath).TrimStart('.');
                WrapperType wrapperType = WrapperFactory.GetFileType(magic, extension);
                IWrapper? wrapper = WrapperFactory.CreateWrapper(wrapperType, stream);

                if (wrapper is null)
                    return new ExtractionResult(false, $"No wrapper could be created for {wrapperType}.");

                if (wrapper is not IExtractable extractable)
                    return new ExtractionResult(false, $"{wrapperType} does not expose extraction support.");

                bool extracted = extractable.Extract(outputDirectory, includeDebug);
                return extracted
                    ? new ExtractionResult(true, $"Extracted '{wrapper.Description()}' to '{outputDirectory}'.")
                    : new ExtractionResult(false, $"Extraction failed for '{wrapper.Description()}'.");
            }
            catch (Exception ex)
            {
                return new ExtractionResult(false, ex.Message);
            }
        }

        private static byte[] ReadMagic(Stream stream)
        {
            byte[] buffer = new byte[Math.Min(16, (int)stream.Length)];
            int totalRead = 0;

            while (totalRead < buffer.Length)
            {
                int read = stream.Read(buffer, totalRead, buffer.Length - totalRead);
                if (read == 0)
                    break;

                totalRead += read;
            }

            if (totalRead == buffer.Length)
                return buffer;

            Array.Resize(ref buffer, totalRead);
            return buffer;
        }
    }
}
