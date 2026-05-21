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
//    public sealed class DhlTrackingClient : IDhlTrackingClient
//    {
//        private readonly HttpClient _http;
//        private readonly DhlTrackingOptions_Old _opt;

//        public DhlTrackingClient(HttpClient http, IOptions<DhlTrackingOptions_Old> opt)
//        {
//            _http = http;
//            _opt = opt.Value;
//        }

//        public async Task<string> GetKnownTrackingAsync(string awb, CancellationToken ct)
//        {
//            var messageTime = System.DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz");
//            static string E(string s) => System.Security.SecurityElement.Escape(s) ?? "";

//            var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
//<req:KnownTrackingRequest xmlns:req=""http://www.dhl.com"" schemaVersion=""1.0"">
//  <Request>
//    <ServiceHeader>
//      <MessageTime>{messageTime}</MessageTime>
//      <MessageReference>{E(_opt.MessageReference)}</MessageReference>
//      <SiteID>{E(_opt.SiteID)}</SiteID>
//      <Password>{E(_opt.Password)}</Password>
//    </ServiceHeader>
//  </Request>
//  <LanguageCode>{E(_opt.LanguageCode)}</LanguageCode>
//  <AWBNumber>{E(awb)}</AWBNumber>
//  <LevelOfDetails>{E(_opt.LevelOfDetails)}</LevelOfDetails>
//  <PiecesEnabled>{E(_opt.PiecesEnabled)}</PiecesEnabled>
//</req:KnownTrackingRequest>";

//            using var content = new StringContent(xml, Encoding.UTF8, "application/xml");
//            using var resp = await _http.PostAsync(_opt.TrackURL, content, ct);
//            var body = await resp.Content.ReadAsStringAsync(ct);
//            resp.EnsureSuccessStatusCode();
//            return body;
//        }
//    }
}
