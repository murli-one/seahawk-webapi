using Microsoft.Extensions.Options;
using SeaHawkServices.Application.Contract;
using SeaHawkServices.Application.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeaHawkServices.Infrastructure.Reports
{
    public sealed class DropboxGateway : IDropboxGateway
    {
        private readonly HttpClient _http;
        private readonly ReportStorageOptions _opt;
        public DropboxGateway(HttpClient http, IOptions<ReportStorageOptions> opt)
        {
            _http = http; _opt = opt.Value;
        }

        public async Task<Stream?> DownloadAsync(string dropboxPath)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/download");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opt.AccessToken);

            var arg = JsonSerializer.Serialize(new { path = dropboxPath }); // must start with '/'
            req.Headers.TryAddWithoutValidation("Dropbox-API-Arg", arg);

            var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var text = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                // Typical: {"error_summary":"path/not_found/...","error":{".tag":"path","path":{".tag":"not_found"}}}
                // Or:     {"error_summary":"path/restricted_content/..."}
                // Or:     {"error_summary":"unsupported_path/..."}
                // Log this so you can see the exact reason:
                Console.Error.WriteLine($"Dropbox download failed ({(int)resp.StatusCode}): {text}");
                return null;
            }

            var net = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var ms = new MemoryStream();
            await net.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;
            resp.Dispose();
            return ms;
        }
        public async Task<string?> FindPdfPathForSampleAsync(string sampleNumber, string root)
        {
            var body = new
            {
                query = sampleNumber,
                options = new { path = root, filename_only = true, max_results = 50 }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/search_v2");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opt.AccessToken);
            req.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var resp = await _http.SendAsync(req).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return null;

            using var json = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(json).ConfigureAwait(false);

            var expected = (sampleNumber + ".pdf").ToLowerInvariant();
            if (doc.RootElement.TryGetProperty("matches", out var matches))
            {
                // Try exact filename first
                foreach (var m in matches.EnumerateArray())
                {
                    var md = m.GetProperty("metadata").GetProperty("metadata");
                    var name = md.GetProperty("name").GetString() ?? "";
                    var path = md.GetProperty("path_display").GetString() ?? "";
                    if (name.Equals(expected, StringComparison.OrdinalIgnoreCase))
                        return path;
                }
                // Fallback: first PDF match
                foreach (var m in matches.EnumerateArray())
                {
                    var md = m.GetProperty("metadata").GetProperty("metadata");
                    var name = md.GetProperty("name").GetString() ?? "";
                    if (name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        return md.GetProperty("path_display").GetString();
                }
            }
            return null;
        }
    }
}

