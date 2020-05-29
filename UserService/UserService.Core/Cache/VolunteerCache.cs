using EnumsNET;
using HelpMyStreet.Cache;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.Core.Cache
{
    public class VolunteerCache : IVolunteerCache
    {
        private readonly IMemDistCache<IEnumerable<CachedVolunteerDto>> _memDistCache;
        private readonly IVolunteersForCacheGetter _volunteersForCacheGetter;

        public VolunteerCache(IMemDistCache<IEnumerable<CachedVolunteerDto>> memDistCache, IVolunteersForCacheGetter volunteersForCacheGetter)
        {
            _memDistCache = memDistCache;
            _volunteersForCacheGetter = volunteersForCacheGetter;
        }

        /// <summary>
        /// Get volunteers using cache. 
        /// </summary>
        public async Task<IEnumerable<CachedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, IsVerifiedType isVerifiedType, CancellationToken cancellationToken)
        {
            // Don't refresh data in cache because this will be refreshed through a Timed trigger.  This is because getting dependencies on a new thread using IServiceScopeFactory throws an error in Azure Functions (Scope disposed{no name, Parent={no name}} is disposed and scoped instances are disposed and no longer available).
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await _memDistCache.GetCachedDataAsync(async (token) => await _volunteersForCacheGetter.GetAllVolunteersAsync(token), CacheKey.AllCachedVolunteerDtos.ToString(), RefreshBehaviour.DontRefreshData, cancellationToken);

            List<CachedVolunteerDto> matchingVolunteers = cachedVolunteerDtos.Where(x => x.VolunteerType.HasAnyFlags(volunteerType) && x.IsVerifiedType.HasAnyFlags(isVerifiedType)).ToList();

            return matchingVolunteers;
        }

    }
}
