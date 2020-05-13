using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;

namespace UserService.Core.PreCalculation
{
    public interface IPrecalculatedVolunteersGetter
    {
        Task<IEnumerable<PrecalculatedVolunteerDto>> GetAllPrecalculatedVolunteersAsync(CancellationToken cancellationToken);
    }
}