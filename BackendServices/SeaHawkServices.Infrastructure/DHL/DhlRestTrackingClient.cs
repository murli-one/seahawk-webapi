using Microsoft.Extensions.Options;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Infrastructure.DHL
{
    public sealed class DhlRestTrackingClient : IDhlTrackingClient
    {
        private readonly HttpClient _http;
        private readonly DhlTrackingOptions _opt;

        public DhlRestTrackingClient(HttpClient http, IOptions<DhlTrackingOptions> opt)
        {
            _http = http;
            _opt = opt.Value;

            // Add default API Key header (MyDHL requirement)
            if (!_http.DefaultRequestHeaders.Contains("DHL-API-Key"))
            {
                _http.DefaultRequestHeaders.Add("DHL-API-Key", _opt.ApiKey); // API Key
            }
        }

        //public async Task<string> GetKnownTrackingAsync(string awb, CancellationToken ct)
        //{
        //    var url = $"https://express.api.dhl.com/mydhlapi/test/shipments/{awb}";

        //    using var request = new HttpRequestMessage(HttpMethod.Get, url);

        //    // Add Authorization Header (Basic = key:secret)
        //    var authString = $"{_opt.ApiKey}:{_opt.ApiSecret}";
        //    var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
        //    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authBase64);

        //    using var response = await _http.SendAsync(request, ct);
        //    var json = await response.Content.ReadAsStringAsync(ct);

        //    response.EnsureSuccessStatusCode();
        //    return json;
        //}
        public async Task<string> GetKnownTrackingAsync(string awb, CancellationToken ct)
        {
            // Ideally: from config, e.g. _opt.BaseUrl = "https://api-eu.dhl.com"
            var baseUrl = _opt.BaseUrl?.TrimEnd('/') ?? "https://api-eu.dhl.com";

            var url = $"{baseUrl}/track/shipments?trackingNumber={awb}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            // DHL Shipment Tracking – Unified API uses API key header
            request.Headers.TryAddWithoutValidation("DHL-API-Key", _opt.ApiKey);

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            // For debugging, log the body if non-success
            if (!response.IsSuccessStatusCode)
            {
               // _log.LogWarning("DHL tracking call failed. Status: {Status} Body: {Body}",
                   // response.StatusCode, body);
            }

            response.EnsureSuccessStatusCode();
            return body;
        }
    }
}
