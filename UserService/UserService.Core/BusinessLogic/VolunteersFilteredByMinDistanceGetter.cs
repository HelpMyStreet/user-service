using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.PreCalculation;

namespace UserService.Core.BusinessLogic
{
    public class VolunteersFilteredByMinDistanceGetter : IVolunteersFilteredByMinDistanceGetter
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IMinDistanceFilter _minDistanceFilter;

        public VolunteersFilteredByMinDistanceGetter(IVolunteerCache volunteerCache, IMinDistanceFilter minDistanceFilter)
        {
            _volunteerCache = volunteerCache;
            _minDistanceFilter = minDistanceFilter;
        }

        public async Task<IEnumerable<PrecalculatedVolunteerDto>> GetVolunteersFilteredByMinDistanceAsync(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<PrecalculatedVolunteerDto> cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);

            IEnumerable<PrecalculatedVolunteerDto> cachedVolunteerDtosFilteredByMinDistance = _minDistanceFilter.FilterByMinDistance(cachedVolunteerDtos, request.MinDistanceBetweenInMetres);

            return cachedVolunteerDtosFilteredByMinDistance;
        }
    }
}
