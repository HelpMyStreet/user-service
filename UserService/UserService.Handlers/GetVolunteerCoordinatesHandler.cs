using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.CoordinatedResetCache;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.BusinessLogic;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Extensions;

namespace UserService.Handlers
{
    public class GetVolunteerCoordinatesHandler : IRequestHandler<GetVolunteerCoordinatesRequest, GetVolunteerCoordinatesResponse>
    {
        private readonly ICoordinatedResetCache _coordinatedResetCache;
        private readonly IVolunteerCache _volunteerCache;
        private readonly IVolunteersFilteredByMinDistanceGetter _volunteersFilteredByMinDistanceGetter;

        public GetVolunteerCoordinatesHandler(ICoordinatedResetCache coordinatedResetCache, IVolunteerCache volunteerCache, IVolunteersFilteredByMinDistanceGetter volunteersFilteredByMinDistanceGetter)
        {
            _coordinatedResetCache = coordinatedResetCache;
            _volunteerCache = volunteerCache;
            _volunteersFilteredByMinDistanceGetter = volunteersFilteredByMinDistanceGetter;
        }

        public async Task<GetVolunteerCoordinatesResponse> Handle(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos;

            // calculating coordinates that have a minimum distance between them is expensive so cache the result
            if (request.MinDistanceBetweenInMetres > 0)
            {
                string key = $"{nameof(CachedVolunteerDto)}_MinDistance_{request.VolunteerType}_{request.IsVerifiedType}";

                cachedVolunteerDtos = await _coordinatedResetCache.GetCachedDataAsync(async (token) => await _volunteersFilteredByMinDistanceGetter.GetVolunteersFilteredByMinDistanceAsync(request, token), key, cancellationToken, CoordinatedResetCacheTime.OnHour);
            }
            else
            {
                cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);
            }

            IReadOnlyList<VolunteerCoordinate> cachedVolunteerDtosWithinBoundary = cachedVolunteerDtos.WhereWithinBoundary(request.SWLatitude, request.SWLongitude, request.NELatitude, request.NELongitude)
                .Select(x => new VolunteerCoordinate()
                {
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Postcode = x.Postcode
                }).ToList();

            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse = new GetVolunteerCoordinatesResponse()
            {
                Coordinates = cachedVolunteerDtosWithinBoundary
            };

            return getVolunteerCoordinatesResponse;
        }

    }
}