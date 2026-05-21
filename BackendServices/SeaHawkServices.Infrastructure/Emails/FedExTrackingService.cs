using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SeaHawkService.Application.Contract;
using Data;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace SeaHawkService.Infrastructure.Emails
{
    //public class FedExTrackingService : IFedExTrackingService
    //{
    //    public async Task<TrackingResult> TrackAsync(string trackingNumber)
    //    {
    //        var request = CreateTrackRequest(trackingNumber);
    //        var service = new TrackService();

    //        var reply = service.track(request);

    //        return new TrackingResult
    //        {
    //            Status = reply.HighestSeverity.ToString(),
    //            Events = reply.CompletedTrackDetails?
    //                .FirstOrDefault()?.TrackDetails?
    //                .FirstOrDefault()?.Events?
    //                .Select(e => $"{e.EventDescription} - {e.Timestamp}")
    //                .ToList()
    //        };
    //    }

    //    private TrackRequest CreateTrackRequest(string trackingNumber)
    //    {
    //        return new TrackRequest
    //        {
    //            WebAuthenticationDetail = new WebAuthenticationDetail
    //            {
    //                UserCredential = new WebAuthenticationCredential
    //                {
    //                    Key = "YourKey",
    //                    Password = "YourPassword"
    //                }
    //            },
    //            ClientDetail = new ClientDetail
    //            {
    //                AccountNumber = "YourAccount",
    //                MeterNumber = "YourMeter"
    //            },
    //            TransactionDetail = new TransactionDetail
    //            {
    //                CustomerTransactionId = "Track Request"
    //            },
    //            Version = new VersionId(),
    //            SelectionDetails = new[]
    //            {
    //                new TrackSelectionDetail
    //                {
    //                    PackageIdentifier = new TrackPackageIdentifier
    //                    {
    //                        Type = TrackIdentifierType.TRACKING_NUMBER_OR_DOORTAG,
    //                        Value = trackingNumber
    //                    }
    //                }
    //            }
    //        };
    //    }
    //}
}
