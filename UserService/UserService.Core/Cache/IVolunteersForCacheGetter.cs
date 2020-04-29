using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;

namespace UserService.Core
{
    public interface IVolunteersForCacheGetter
    {
        Task<IEnumerable<CachedVolunteerDto>> GetAllVolunteersAsync(CancellationToken cancellationToken);
    }
}