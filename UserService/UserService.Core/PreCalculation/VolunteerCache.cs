using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using HelpMyStreet.Utils.CoordinatedResetCache;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.Core.PreCalculation
{
    public class VolunteerCache : IVolunteerCache
    {
        private readonly ICoordinatedResetCache _coordinatedResetCache;
        private readonly IPrecalculatedVolunteersGetter _precalculatedVolunteersGetter;

        public VolunteerCache(ICoordinatedResetCache coordinatedResetCache, IPrecalculatedVolunteersGetter precalculatedVolunteersGetter)
        {
            _coordinatedResetCache = coordinatedResetCache;
            _precalculatedVolunteersGetter = precalculatedVolunteersGetter;
        }

        /// <summary>
        /// Get volunteers using cache. 
        /// </summary>
        public async Task<IEnumerable<PrecalculatedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, IsVerifiedType isVerifiedType, CancellationToken cancellationToken)
        {
          IEnumerable<PrecalculatedVolunteerDto> cachedVolunteerDtos = await _coordinatedResetCache.GetCachedDataAsync(async () => await _precalculatedVolunteersGetter.GetAllPrecalculatedVolunteersAsync(cancellationToken), "AllCachedVolunteerDtos", CoordinatedResetCacheTime.OnHour);

            List<PrecalculatedVolunteerDto> matchingVolunteers = cachedVolunteerDtos.Where(x => x.VolunteerType.HasAnyFlags(volunteerType) && x.IsVerifiedType.HasAnyFlags(isVerifiedType)).ToList();

            return matchingVolunteers;
        }

    }
}
