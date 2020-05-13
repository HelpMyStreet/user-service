using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.Core.PreCalculation
{
    public interface IVolunteerCache
    {
        Task<IEnumerable<PrecalculatedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, IsVerifiedType isVerifiedType, CancellationToken cancellationToken);
    }
}