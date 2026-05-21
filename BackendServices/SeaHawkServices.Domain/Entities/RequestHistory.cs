using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    public enum RequestHistoryAction
    {
        Created,
        StatusChanged,
        TrackingUpdated,
        CommentAdded,
        CanceledByClient,
        CanceledByAdmin,
        APIDispatch,
        ErrorOccured
    }

    public class RequestHistory
    {
        public int Id { get; set; }

        /// <summary>
        /// ID of the request (SampleCollection.Id, SamplingKit.Id, etc.)
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// e.g. "SampleCollection", "SamplingKit"
        /// </summary>
        public string RequestType { get; set; }

        public VesselDetail? VesselDetail { get; set; }
        public int? VesselDetailId { get; set; }

      
        /// <summary>
        /// One of RequestHistoryAction (stored as string)
        /// </summary>
        public RequestHistoryAction Action { get; set; }

        /// <summary>
        /// Username or email of user who performed the action
        /// </summary>
        public string? PerformedBy { get; set; }

        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Free-text notes or comments
        /// </summary>
        public string? Notes { get; set; }

        // --- Extra fields to satisfy client’s log spec ---

        /// <summary>
        /// Previous status (e.g. Pending)
        /// </summary>
        public string? OldStatus { get; set; }

        /// <summary>
        /// New status after change (e.g. Approved)
        /// </summary>
        public string? NewStatus { get; set; }

        /// <summary>
        /// Tracking / AWB number at the time of this action
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Courier used at this step (FedEx, DHL, etc.)
        /// </summary>
        public string? Courier { get; set; }

        /// <summary>
        /// API dispatch reference / response ID / booking ID
        /// </summary>
        public string? ApiDispatchReference { get; set; }

    }

    /// <summary>
    /// Central place for all RequestHistory.Action values
    /// </summary>
}
