using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SeaHawkServices.Web.ViewModels
{
    public interface ITypesenseSearchService
    {
        Task<List<string>> GetSuggestionsAsync(string field, string query);
        Task UpsertAsync(object doc);
        Task BulkUpsertAsync(IEnumerable<object> docs);
    }

    public class TypesenseSearchService : ITypesenseSearchService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public TypesenseSearchService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        private (string BaseUrl, string ApiKey) GetConfig()
        {
            var baseUrl = _config["Typesense:BaseUrl"]?.TrimEnd('/');
            var apiKey = _config["Typesense:AdminApiKey"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Typesense:BaseUrl is missing in configuration.");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Typesense:AdminApiKey is missing in configuration.");

            return (baseUrl, apiKey);
        }

        private string ApplyHeaders()
        {
            var (baseUrl, apiKey) = GetConfig();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", apiKey);
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return baseUrl;
        }

        public async Task<List<string>> GetSuggestionsAsync(string field, string query)
        {
            var baseUrl = ApplyHeaders();
            var collection = "analysis_results";

            var url =
                $"{baseUrl}/collections/{collection}/documents/search" +
                $"?q={Uri.EscapeDataString(query)}" +
                $"&query_by={Uri.EscapeDataString(field)}" +
                $"&per_page=10";

            var res = await _http.GetAsync(url);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("hits")
                .EnumerateArray()
                .Select(h => h.GetProperty("document").GetProperty(field).GetString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()!;
        }

        public async Task UpsertAsync(object doc)
        {
            var baseUrl = ApplyHeaders();
            var collection = "analysis_results";

            var url = $"{baseUrl}/collections/{collection}/documents?action=upsert";
            var body = JsonSerializer.Serialize(doc);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var res = await _http.PostAsync(url, content);
            res.EnsureSuccessStatusCode();
        }

        public async Task BulkUpsertAsync(IEnumerable<object> docs)
        {
            var baseUrl = ApplyHeaders();
            var collection = "analysis_results";

            var jsonl = string.Join("\n", docs.Select(d => JsonSerializer.Serialize(d)));
            var content = new StringContent(jsonl, Encoding.UTF8, "application/x-ndjson");

            var url = $"{baseUrl}/collections/{collection}/documents/import?action=upsert";
            var res = await _http.PostAsync(url, content);
            res.EnsureSuccessStatusCode();

            var responseText = await res.Content.ReadAsStringAsync();
            var lines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var failed = new List<string>();
            foreach (var line in lines)
            {
                using var rdoc = JsonDocument.Parse(line);
                var root = rdoc.RootElement;

                if (root.TryGetProperty("success", out var s) &&
                    s.ValueKind == JsonValueKind.False)
                {
                    var err = root.TryGetProperty("error", out var e) ? e.GetString() : "Unknown error";
                    failed.Add(err ?? "Unknown error");
                }
            }

            if (failed.Any())
                throw new InvalidOperationException("Typesense bulk import had failures: " + string.Join(" | ", failed.Take(5)));
        }
    }
}

