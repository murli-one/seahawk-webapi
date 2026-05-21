using Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Options;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.DHL;
using SeaHawkServices.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Application.Services.Implementation
{
    public sealed class ShipmentTrackingService : IShipmentTrackingService
    {
        private readonly IDhlTrackingClient _dhl;
        private readonly IFedExTrackingClient _fedex;
        private readonly ILogger<ShipmentTrackingService> _log;

        public ShipmentTrackingService(IDhlTrackingClient dhl, IFedExTrackingClient fedex, ILogger<ShipmentTrackingService> log)
        {
            _dhl = dhl;
            _fedex = fedex;
            _log = log;
        }

        public async Task<TrackingResult> TrackAsync(CourierProvider courier, string awb, CancellationToken ct)
        {
            try
            {
                switch (courier)
                {
                    case CourierProvider.DHL:
                        {
                            //var xmlResp = await _dhl.GetKnownTrackingAsync(awb, ct);
                            //return DhlXmlParser.ParseTrackingResponse(xmlResp);
                            var jsonResp = await _dhl.GetKnownTrackingAsync(awb, ct);
                            return DhlJsonParser.ParseJson(jsonResp);

                        }

                    case CourierProvider.FedEx:
                        {
                            var xmlResp = await _fedex.TrackAsync(awb, ct);
                            //return FedExXmlParser.ParseTrackingResponse(xmlResp);
                            return FedExXmlParser.ParseTrackingResponse(xmlResp);
                        }

                    default:
                        return new TrackingResult
                        {
                            Success = false,
                            Error = "Unsupported courier."
                        };
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Tracking failed for {Courier} {AWB}", courier, awb);
                return new TrackingResult
                {
                    Success = false,
                    Error = "Tracking call failed."
                };
            }
        }

    }
}
