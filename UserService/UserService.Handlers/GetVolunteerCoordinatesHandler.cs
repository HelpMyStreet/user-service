using HelpMyStreet.Utils.CoordinatedResetCache;
using MediatR;
using System.Collections.Generic;
using System.Linq;
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

            IReadOnlyList<VolunteerCoordinate> cachedVolunteerDtosWithinBoundary;

            // calculating coordinates that have a minimum distance between them is expensive so cache the result
            if (request.MinDistanceBetweenInMetres > 0)
            {
                // round up to nearest 2000 metres to prevent repeated calculation of indistinguishable minimum distances and cache taking too much memory
                request.MinDistanceBetweenInMetres = request.MinDistanceBetweenInMetres.RoundUpToNearest(2000);
                string key = $"{nameof(CachedVolunteerDto)}_MinDistance_{request.MinDistanceBetweenInMetres}_{request.VolunteerType}_{request.IsVerifiedType}";

                cachedVolunteerDtos = await _coordinatedResetCache.GetCachedDataAsync(async (token) => await _volunteersFilteredByMinDistanceGetter.GetVolunteersFilteredByMinDistanceAsync(request, token), key, cancellationToken, CoordinatedResetCacheTime.OnHour);

                cachedVolunteerDtosWithinBoundary = cachedVolunteerDtos.WhereWithinBoundary(request.SWLatitude, request.SWLongitude, request.NELatitude, request.NELongitude)
                    .Select(x => new VolunteerCoordinate()
                    {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        Postcode = x.Postcode,
                        // fields are temporarily null until the grid aggregation functionality is implemented
                        NumberOfHelpers = null,
                        NumberOfStreetChampions = null
                    }).ToList();
            }
            else
            {
                cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);

                cachedVolunteerDtosWithinBoundary = cachedVolunteerDtos.WhereWithinBoundary(request.SWLatitude, request.SWLongitude, request.NELatitude, request.NELongitude)
                   .GroupBy(x => new { x.Postcode, x.Latitude, x.Longitude })
                   .Select(x => new VolunteerCoordinate()
                   {
                       Latitude = x.Key.Latitude,
                       Longitude = x.Key.Longitude,
                       Postcode = x.Key.Postcode,
                       NumberOfHelpers = x.Count(y => y.VolunteerType == VolunteerType.Helper),
                       NumberOfStreetChampions = x.Count(y => y.VolunteerType == VolunteerType.StreetChampion),
                   }).ToList();

            }



            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse = new GetVolunteerCoordinatesResponse()
            {
                Coordinates = cachedVolunteerDtosWithinBoundary
            };

            return getVolunteerCoordinatesResponse;
        }

    }
}