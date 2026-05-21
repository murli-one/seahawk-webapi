using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Options;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Contract;
using SeaHawkServices.Application.Options;
using SeaHawkServices.Application.Services.Interface;
using System.Text.RegularExpressions;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class SampleReportService : ISampleReportService
    {
        private static readonly Regex SafeSampleNumber = new("^[A-Za-z0-9._/-]+$", RegexOptions.Compiled);

        private readonly ISampleReportRepository _repo;
        private readonly IDropboxGateway _dropbox;
        private readonly ReportStorageOptions _opt;
        private readonly DropboxTokenService _tokenService;

        public SampleReportService(
            ISampleReportRepository repo,
            IDropboxGateway dropbox,
            IOptions<ReportStorageOptions> opt,
            DropboxTokenService tokenService)
        {
            _repo = repo;
            _dropbox = dropbox;
            _opt = opt.Value;
            _tokenService = tokenService;
        }

        public async Task<(string FileName, Stream Content)?> GetPdfAsync(string sampleNumber)
        {
            if (string.IsNullOrWhiteSpace(sampleNumber))
                return null;

            sampleNumber = sampleNumber.Trim();

            if (!SafeSampleNumber.IsMatch(sampleNumber))
                return null;

            var token = await _tokenService.GetAccessTokenAsync();
            using var dbx = new DropboxClient(token);

            var rootPath = NormalizeDropboxPath(_opt.ReportsRoot);

            var match = await FindMatchingPdfAsync(dbx, rootPath, sampleNumber);
            if (match == null)
                return null;

            var download = await dbx.Files.DownloadAsync(match.PathLower);
            var bytes = await download.GetContentAsByteArrayAsync();
            var stream = new MemoryStream(bytes);

            return (match.Name, stream);
        }

        private async Task<FileMetadata?> FindMatchingPdfAsync(DropboxClient dbx, string rootPath, string sampleNumber)
        {
            try
            {
                var result = await dbx.Files.ListFolderAsync(rootPath, recursive: true);

                while (true)
                {
                    foreach (var entry in result.Entries)
                    {
                        if (!entry.IsFile)
                            continue;

                        var file = entry.AsFile;

                        if (!file.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (IsMatch(file.Name, sampleNumber))
                            return file;
                    }

                    if (!result.HasMore)
                        break;

                    result = await dbx.Files.ListFolderContinueAsync(result.Cursor);
                }
            }
            catch (ApiException<ListFolderError> ex)
            {
                Console.WriteLine($"Dropbox folder read error for '{rootPath}': {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dropbox unexpected error: {ex.Message}");
            }

            return null;
        }

        private static bool IsMatch(string fileName, string sampleNumber)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName).Trim();

            if (nameWithoutExtension.Equals(sampleNumber, StringComparison.OrdinalIgnoreCase))
                return true;

            if (nameWithoutExtension.EndsWith(" - " + sampleNumber, StringComparison.OrdinalIgnoreCase))
                return true;

            if (nameWithoutExtension.EndsWith(sampleNumber, StringComparison.OrdinalIgnoreCase))
                return true;

            return Regex.IsMatch(
                nameWithoutExtension,
                $@"(^|[^A-Za-z0-9]){Regex.Escape(sampleNumber)}($|[^A-Za-z0-9])",
                RegexOptions.IgnoreCase);
        }

        private static string NormalizeDropboxPath(string? path)
        {
            path = (path ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return string.Empty;

            if (!path.StartsWith("/"))
                path = "/" + path;

            return path.TrimEnd('/');
        }
    }
}