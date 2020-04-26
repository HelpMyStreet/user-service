using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;

namespace UserService.Handlers
{
    public class GetHelperCoordsByPostcodeAndRadiusGetter : IGetHelperCoordsByPostcodeAndRadiusGetter
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly IAddressService _addressService;

        public GetHelperCoordsByPostcodeAndRadiusGetter(IVolunteerCache volunteerCache, IDistanceCalculator distanceCalculator, IAddressService addressService)
        {
            _volunteerCache = volunteerCache;
            _distanceCalculator = distanceCalculator;
            _addressService = addressService;
        }

        public async Task<GetVolunteerCoordinatesResponse> GetHelperCoordsByPostcodeAndRadius(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);
            
            HashSet<VolunteerCoordinate> volunteerCoordinates = new HashSet<VolunteerCoordinate>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
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

                    if (request.MinDistanceBetweenInMetres == null || !hasCoordBeenAdded)
                    {
                        hasCoordBeenAdded = true;
                        volunteerCoordinates.Add(volunteerCoordinate);
                    }
                    else
                    {
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

            sw.Stop();
            Debug.WriteLine($"Calculating distances took: {sw.ElapsedMilliseconds}");
            Debug.WriteLine($"Count: {volunteerCoordinates.Count}");
            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse = new GetVolunteerCoordinatesResponse()
            {
                Coordinates = volunteerCoordinates.ToList()
            };

            return getVolunteerCoordinatesResponse;
        }
       
    }
}
