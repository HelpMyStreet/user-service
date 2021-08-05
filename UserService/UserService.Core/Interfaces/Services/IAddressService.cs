using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<GetPostcodeCoordinatesResponse> GetPostcodeCoordinatesAsync(GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest, CancellationToken cancellationToken);

        Task<bool> IsValidPostcode(string postcode, CancellationToken cancellationToken);
    }
}