using Microsoft.Extensions.Options;
using SeaHawkServices.Application.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Implementation
{
    public sealed class DropboxTokenService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ReportStorageOptions _opt;

        private string? _accessToken;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

        private static readonly SemaphoreSlim _lock = new(1, 1);

        public DropboxTokenService(IHttpClientFactory factory, IOptions<ReportStorageOptions> opt)
        {
            _factory = factory;
            _opt = opt.Value;

            // If you want to start with configured AccessToken (until expired)
            _accessToken = string.IsNullOrWhiteSpace(_opt.AccessToken) ? null : _opt.AccessToken;
        }

        public bool CanRefresh => !string.IsNullOrWhiteSpace(_opt.RefreshToken);

        public async Task<string> GetAccessTokenAsync()
        {
            // If we don't have refresh token, we can ONLY use AccessToken (and it may expire)
            if (!CanRefresh)
            {
                if (string.IsNullOrWhiteSpace(_opt.AccessToken))
                    throw new InvalidOperationException("Dropbox RefreshToken not configured and AccessToken is empty.");
                return _opt.AccessToken!;
            }

            // refresh 60s early
            if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _expiresAt.AddSeconds(-60))
                return _accessToken!;

            await RefreshAsync();
            return _accessToken!;
        }

        public async Task RefreshAsync()
        {
            if (!CanRefresh)
                throw new InvalidOperationException("Dropbox RefreshToken is not configured.");

            await _lock.WaitAsync();
            try
            {
                // double check after lock
                if (!string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _expiresAt.AddSeconds(-60))
                    return;

                var http = _factory.CreateClient();
                using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token")
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["grant_type"] = "refresh_token",
                        ["refresh_token"] = _opt.RefreshToken,
                        ["client_id"] = _opt.AppKey,
                        ["client_secret"] = _opt.AppSecret
                    })
                };

                using var res = await http.SendAsync(req).ConfigureAwait(false);
                var json = await res.Content.ReadAsStringAsync().ConfigureAwait(false);

                res.EnsureSuccessStatusCode();

                using var doc = JsonDocument.Parse(json);
                _accessToken = doc.RootElement.GetProperty("access_token").GetString();
                var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
                _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
