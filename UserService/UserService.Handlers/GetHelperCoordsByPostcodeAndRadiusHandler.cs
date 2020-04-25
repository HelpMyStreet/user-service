using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Contracts;
using HelpMyStreet.Utils.Utils;
using UserService.Core;
using UserService.Core.Contracts;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;

namespace UserService.Handlers
{
    public class GetHelperCoordsByPostcodeAndRadiusHandler : IRequestHandler<GetHelperCoordsByPostcodeAndRadiusRequest, GetHelperCoordsByPostcodeAndRadiusResponse>
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly IAddressService _addressService;

        public GetHelperCoordsByPostcodeAndRadiusHandler(IVolunteerCache volunteerCache, IDistanceCalculator distanceCalculator, IAddressService addressService)
        {
            _volunteerCache = volunteerCache;
            _distanceCalculator = distanceCalculator;
            _addressService = addressService;
        }


        public async Task<GetHelperCoordsByPostcodeAndRadiusResponse> Handle(GetHelperCoordsByPostcodeAndRadiusRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            GetPostcodeCoordinatesRequest getPostcodeCoordsRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { request.Postcode }
            };

            Task<GetPostcodeCoordinatesResponse> postcodeCoordsTask = _addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordsRequest, cancellationToken);
            Task<IEnumerable<CachedVolunteerDto>> cachedVolunteerDtosTask = _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);

            await Task.WhenAll(postcodeCoordsTask, cachedVolunteerDtosTask);

            GetPostcodeCoordinatesResponse postcodeCoords = await postcodeCoordsTask;
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await cachedVolunteerDtosTask;

            PostcodeCoordinate postcodeCoordsToCompareTo = postcodeCoords.PostcodeCoordinates.FirstOrDefault();

            List<VolunteerCoordinate> volunteerCoordinates = new List<VolunteerCoordinate>();

            foreach (CachedVolunteerDto cachedVolunteerDto in cachedVolunteerDtos)
            {
                double distance = _distanceCalculator.GetDistanceInMetres(postcodeCoordsToCompareTo.Latitude, postcodeCoordsToCompareTo.Longitude, cachedVolunteerDto.Latitude, cachedVolunteerDto.Longitude);

                bool isWithinSupportRadius = distance <= request.RadiusInMetres;

                if (isWithinSupportRadius)
                {
                    VolunteerCoordinate volunteerCoordinate = new VolunteerCoordinate()
                    {
                        Latitude = cachedVolunteerDto.Latitude,
                        Longitude = cachedVolunteerDto.Longitude,
                        IsVerified = cachedVolunteerDto.IsVerifiedType == IsVerifiedType.IsVerified,
                        VolunteerType = cachedVolunteerDto.VolunteerType
                    };

                    volunteerCoordinates.Add(volunteerCoordinate);
                }
            }
            GetHelperCoordsByPostcodeAndRadiusResponse getHelperCoordsByPostcodeAndRadiusResponse = new GetHelperCoordsByPostcodeAndRadiusResponse()
            {
                Coordinates = volunteerCoordinates
            };

            return getHelperCoordsByPostcodeAndRadiusResponse;
        }
    }
}
