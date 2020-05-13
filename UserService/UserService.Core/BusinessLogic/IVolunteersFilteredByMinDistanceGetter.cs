using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.Core.BusinessLogic
{
    public interface IVolunteersFilteredByMinDistanceGetter
    {
        Task<IEnumerable<PrecalculatedVolunteerDto>> GetVolunteersFilteredByMinDistanceAsync(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken);
    }
}