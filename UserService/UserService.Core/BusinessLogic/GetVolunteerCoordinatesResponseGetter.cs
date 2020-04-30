using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Utils;

namespace UserService.Core.BusinessLogic
{
    public class GetVolunteerCoordinatesResponseGetter : IGetVolunteerCoordinatesResponseGetter
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IDistanceCalculator _distanceCalculator;

        public GetVolunteerCoordinatesResponseGetter(IVolunteerCache volunteerCache, IDistanceCalculator distanceCalculator)
        {
            _volunteerCache = volunteerCache;
            _distanceCalculator = distanceCalculator;
        }

        public async Task<GetVolunteerCoordinatesResponse> GetVolunteerCoordinates(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);

            List<VolunteerCoordinate> volunteerCoordinates = new List<VolunteerCoordinate>();

            bool hasCoordBeenAdded = false;
            foreach (CachedVolunteerDto cachedVolunteerDto in cachedVolunteerDtos)
            {
                bool isWithinSupportRadius;
                if (request.RadiusInMetres == 0)
                {
                    isWithinSupportRadius = true;
                }
                else
                {
                    double distance = _distanceCalculator.GetDistanceInMetres(request.Latitude, request.Longitude, cachedVolunteerDto.Latitude, cachedVolunteerDto.Longitude);

                    isWithinSupportRadius = distance <= request.RadiusInMetres;
                }

                if (isWithinSupportRadius)
                {
                    VolunteerCoordinate volunteerCoordinate = new VolunteerCoordinate()
                    {
                        Latitude = cachedVolunteerDto.Latitude,
                        Longitude = cachedVolunteerDto.Longitude,
                        IsVerified = cachedVolunteerDto.IsVerifiedType == IsVerifiedType.IsVerified,
                        VolunteerType = cachedVolunteerDto.VolunteerType
                    };

                    if (request.MinDistanceBetweenInMetres == 0 || !hasCoordBeenAdded)
                    {
                        hasCoordBeenAdded = true;
                        volunteerCoordinates.Add(volunteerCoordinate);
                    }
                    else
                    {
                        // checks to see if there aren't any coordinates already within the min distance of the coordinates in the request 
                        bool isTooNearToCoords = false;
                        foreach (VolunteerCoordinate existingVolunteerCoordinate in volunteerCoordinates)
                        {
                            double distanceBetweenOtherCoords = _distanceCalculator.GetDistanceInMetres(existingVolunteerCoordinate.Latitude, existingVolunteerCoordinate.Longitude, volunteerCoordinate.Latitude, volunteerCoordinate.Longitude);
                            if (distanceBetweenOtherCoords < request.MinDistanceBetweenInMetres)
                            {
                                isTooNearToCoords = true;
                                break;
                            }
                        }

                        if (!isTooNearToCoords)
                        {
                            volunteerCoordinates.Add(volunteerCoordinate);
                        }
                    }
                }

            }

            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse = new GetVolunteerCoordinatesResponse()
            {
                Coordinates = volunteerCoordinates.ToList()
            };
            
            return getVolunteerCoordinatesResponse;
        }

    }
}
