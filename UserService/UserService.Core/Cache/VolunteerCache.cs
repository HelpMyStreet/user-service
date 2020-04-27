using EnumsNET;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.CoordinatedResetCache;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.Core.Cache
{
    public class VolunteerCache : IVolunteerCache
    {
        private readonly ICoordinatedResetCache _coordinatedResetCache;
        private readonly IVolunteersForCacheGetter _volunteersForCacheGetter;

        public VolunteerCache(ICoordinatedResetCache coordinatedResetCache, IVolunteersForCacheGetter volunteersForCacheGetter)
        {
            _coordinatedResetCache = coordinatedResetCache;
            _volunteersForCacheGetter = volunteersForCacheGetter;
        }

        /// <summary>
        /// Get volunteers using cache. 
        /// </summary>
        public async Task<IEnumerable<CachedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, IsVerifiedType isVerifiedType, CancellationToken cancellationToken)
        {
          IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await _coordinatedResetCache.GetCachedDataAsync(async () => await _volunteersForCacheGetter.GetAllVolunteersAsync(cancellationToken), "AllCachedVolunteerDtos", CoordinatedResetCacheTime.OnHour);

            List<CachedVolunteerDto> matchingVolunteers = cachedVolunteerDtos.Where(x => x.VolunteerType.HasAnyFlags(volunteerType) && x.IsVerifiedType.HasAnyFlags(isVerifiedType)).ToList();

            return matchingVolunteers;
        }

    }
}
