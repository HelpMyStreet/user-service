using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;

namespace UserService.Handlers
{
    public interface IGetVolunteerCoordinatesResponseGetter
    {
        Task<GetVolunteerCoordinatesResponse> GetVolunteerCoordinates(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken);
    }
}