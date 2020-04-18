using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;

namespace UserService.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<IsPostcodeWithinRadiiResponse> IsPostcodeWithinRadiiAsync(IsPostcodeWithinRadiiRequest isPostcodeWithinRadiiRequest, CancellationToken cancellationToken);
    }
}