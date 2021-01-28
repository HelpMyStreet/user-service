using HelpMyStreet.Contracts.UserService.Request;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;

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

        public async Task<IEnumerable<CachedVolunteerDto>> GetVolunteersFilteredByMinDistanceAsync(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, cancellationToken);

            IEnumerable<CachedVolunteerDto> cachedVolunteerDtosFilteredByMinDistance = _minDistanceFilter.FilterByMinDistance(cachedVolunteerDtos, request.MinDistanceBetweenInMetres);

            return cachedVolunteerDtosFilteredByMinDistance;
        }
    }
}
