using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;

namespace UserService.Core.BusinessLogic
{
    public interface IGetVolunteerCoordinatesResponseGetter
    {
        Task<GetVolunteerCoordinatesResponse> GetVolunteerCoordinates(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken);
    }
}