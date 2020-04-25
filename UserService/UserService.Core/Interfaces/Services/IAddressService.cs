using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Contracts;

namespace UserService.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<IsPostcodeWithinRadiiResponse> IsPostcodeWithinRadiiAsync(IsPostcodeWithinRadiiRequest isPostcodeWithinRadiiRequest, CancellationToken cancellationToken);

        Task<GetPostcodeCoordinatesResponse> GetPostcodeCoordinatesAsync(GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest, CancellationToken cancellationToken);
    }
}