using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using UserService.Core;
using UserService.Core.BusinessLogic;
using UserService.Core.Dto;
using UserService.Core.Extensions;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Enums;

namespace UserService.Handlers
{
    public class GetVolunteerCoordinatesHandler : IRequestHandler<GetVolunteerCoordinatesRequest, GetVolunteerCoordinatesResponse>
    {
        private readonly IMemDistCache<IEnumerable<CachedVolunteerDto>> _memDistCache;
        private readonly IVolunteerCache _volunteerCache;
        private readonly IVolunteersFilteredByMinDistanceGetter _volunteersFilteredByMinDistanceGetter;

        public GetVolunteerCoordinatesHandler(IMemDistCache<IEnumerable<CachedVolunteerDto>> memDistCache, IVolunteerCache volunteerCache, IVolunteersFilteredByMinDistanceGetter volunteersFilteredByMinDistanceGetter)
        {
            _memDistCache = memDistCache;
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
                string key = $"{nameof(CachedVolunteerDto)}_MinDistance_{request.MinDistanceBetweenInMetres}_{request.VolunteerType}_{request.IsVerifiedType}_WithCreationDate";

                cachedVolunteerDtos = await _memDistCache.GetCachedDataAsync(async (token) => await _volunteersFilteredByMinDistanceGetter.GetVolunteersFilteredByMinDistanceAsync(request, token), key, RefreshBehaviour.DontWaitForFreshData, cancellationToken);

                cachedVolunteerDtosWithinBoundary = cachedVolunteerDtos.WhereWithinBoundary(request.SWLatitude, request.SWLongitude, request.NELatitude, request.NELongitude)
                    .Select(x => new VolunteerCoordinate()
                    {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        Postcode = x.Postcode,
                        CreatedDate = null,
                        // fields are temporarily null until the grid aggregation functionality is implemented
                        NumberOfHelpers = null,
                        NumberOfStreetChampions = null
                    })
                    .OrderBy(x => x.CreatedDate)
                    .ToList();
            }
            else
            {
                cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, cancellationToken);

                cachedVolunteerDtosWithinBoundary = cachedVolunteerDtos.WhereWithinBoundary(request.SWLatitude, request.SWLongitude, request.NELatitude, request.NELongitude)
                   .GroupBy(x => new { x.Postcode, x.Latitude, x.Longitude })
                   .Select(x => new VolunteerCoordinate()
                   {
                       Latitude = x.Key.Latitude,
                       Longitude = x.Key.Longitude,
                       Postcode = x.Key.Postcode,
                       CreatedDate = x.Min(dm => dm.CreationDate),
                       NumberOfHelpers = x.Count(y => y.VolunteerType == VolunteerType.Helper),
                       NumberOfStreetChampions = x.Count(y => y.VolunteerType == VolunteerType.StreetChampion),
                   }).ToList();

            }

            int numberOfStreetChampions = cachedVolunteerDtosWithinBoundary.Sum(x => x.NumberOfStreetChampions) ?? 0;
            int numberOfHelpers = cachedVolunteerDtosWithinBoundary.Sum(x => x.NumberOfHelpers) ?? 0;



            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse = new GetVolunteerCoordinatesResponse()
            {
                Coordinates = cachedVolunteerDtosWithinBoundary,
                NumberOfStreetChampions = numberOfStreetChampions,
                NumberOfHelpers = numberOfHelpers,
                TotalNumberOfVolunteers = numberOfStreetChampions + numberOfHelpers
            };

            return getVolunteerCoordinatesResponse;
        }

    }
}