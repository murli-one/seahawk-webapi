using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IFedExShippingService
    {
        Task<FedExShipmentResult> CreateShipmentSampleOrderAsync(SampleCollectionRequestVM request,CancellationToken ct);
        Task<FedExShipmentResult> CreateShipmentSampleKitOrderAsync(SampleCollectionRequestVM request,CancellationToken ct);
        Task<AddressValidationResponseDto> ValidateAddressAsync(AddressValidationRequestDto dto, CancellationToken ct);

        Task<byte[]> DownloadDocumentAsync(string documentUrl, CancellationToken ct);

        Task<string> GetAccessTokenAsync(System.Threading.CancellationToken ct);
    }
}
