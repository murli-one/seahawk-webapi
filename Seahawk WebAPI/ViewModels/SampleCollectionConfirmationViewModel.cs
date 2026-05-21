namespace SeaHawkServices.Web.ViewModels
{
    public class SampleCollectionConfirmationViewModel
    {
        public string CollectionReference { get; set; }
        public string? TrackingNumber { get; set; }
        public string? LabelUrl { get; set; }
        public string? InvoiceUrl { get; set; }
        public string? MsdsUrl { get; set; }
        public bool IsPendingApproval { get; internal set; }
    }

}
