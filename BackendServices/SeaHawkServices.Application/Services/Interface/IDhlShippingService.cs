using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IDhlShippingService
    {
        Task<DhlShipmentResult> CreateShipmentSampleOrderAsync(
            SampleCollectionRequestVM request,
            CancellationToken ct);

        Task<AddressValidationResponseDto> ValidateAddressAsync(AddressValidationRequestDto dto, CancellationToken ct);
    }
}
