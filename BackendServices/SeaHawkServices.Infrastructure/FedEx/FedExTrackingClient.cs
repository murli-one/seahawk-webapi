using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeaHawkServices.Infrastructure.FedEx
{
    public sealed class FedExTrackingClient : IFedExTrackingClient
    {
        private readonly HttpClient _http;
        private readonly FedExSettings _settings;
        private readonly ILogger<FedExTrackingClient> _log;

        public FedExTrackingClient(
            HttpClient http,
            IOptions<FedExSettings> options,
            ILogger<FedExTrackingClient> log)
        {
            _http = http;
            _settings = options.Value;
            _log = log;
        }

            public async Task<string> TrackAsync(string trackingNumber, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(trackingNumber))
                    throw new ArgumentException("Tracking number is required", nameof(trackingNumber));

                // 1) Get OAuth 2.0 access token
                var accessToken = await GetAccessTokenAsync(ct);

                // 2) Build Track API URL
                var trackUrl = string.IsNullOrWhiteSpace(_settings.TrackUrl)
                    ? $"{_settings.OAuthBaseUrl.TrimEnd('/')}/track/v1/trackingnumbers"
                    : _settings.TrackUrl;

                // 3) Build request payload for Track API
                var payload = new
                {
                    includeDetailedScans = true,
                    trackingInfo = new[]
                    {
                        new
                        {
                            trackingNumberInfo = new
                            {
                                trackingNumber = trackingNumber
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);

                var request = new HttpRequestMessage(HttpMethod.Post, trackUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                _log.LogInformation("Sending FedEx REST Track request for {Tracking} to {Url}", trackingNumber, trackUrl);

                var response = await _http.SendAsync(request, ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    _log.LogError(ex,
                        "FedEx Track API call failed. StatusCode={StatusCode}, Body={Body}",
                        (int)response.StatusCode,
                        content);

                    throw;
                }

                // We still return the raw payload string so FedExXmlParser can handle it.
                return content;
            }

        private async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            var tokenUrl = $"{_settings.OAuthBaseUrl.TrimEnd('/')}/oauth/token";

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.ApiKey,
                ["client_secret"] = _settings.ApiSecret
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
            {
                Content = new FormUrlEncodedContent(form)
            };

            _log.LogInformation("Requesting FedEx OAuth token from {Url}", tokenUrl);

            var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _log.LogError(ex,
                    "FedEx OAuth token request failed. StatusCode={StatusCode}, Body={Body}",
                    (int)response.StatusCode,
                    body);

                throw;
            }

            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("access_token", out var tokenProp))
            {
                _log.LogError("FedEx OAuth token response missing access_token. Body={Body}", body);
                throw new InvalidOperationException("FedEx OAuth token response missing access_token.");
            }

            var token = tokenProp.GetString();
            if (string.IsNullOrWhiteSpace(token))
            {
                _log.LogError("FedEx OAuth token is empty. Body={Body}", body);
                throw new InvalidOperationException("FedEx OAuth token is empty.");
            }

            return token;
        }
    }
}
