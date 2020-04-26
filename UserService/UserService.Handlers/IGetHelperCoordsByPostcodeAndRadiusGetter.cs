using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;

namespace UserService.Handlers
{
    public interface IGetHelperCoordsByPostcodeAndRadiusGetter
    {
        Task<GetVolunteerCoordinatesResponse> GetHelperCoordsByPostcodeAndRadius(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken);
    }
}