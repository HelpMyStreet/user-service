using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Cache;
using UserService.Core.Domains.Entities;

namespace UserService.Handlers
{
    public class GetVolunteerCoordinatesHandler : IRequestHandler<GetVolunteerCoordinatesRequest, GetVolunteerCoordinatesResponse>
    {
        private readonly IGetHelperCoordsByPostcodeAndRadiusGetter _getHelperCoordsByPostcodeAndRadiusGetter;
        private readonly ICoordinatedResetCache _coordinatedResetCache;

        public GetVolunteerCoordinatesHandler(IGetHelperCoordsByPostcodeAndRadiusGetter helperCoordsByPostcodeAndRadiusGetter, ICoordinatedResetCache coordinatedResetCache)
        {
            _getHelperCoordsByPostcodeAndRadiusGetter = helperCoordsByPostcodeAndRadiusGetter;
            _coordinatedResetCache = coordinatedResetCache;
        }

        public async Task<GetVolunteerCoordinatesResponse> Handle(GetVolunteerCoordinatesRequest request, CancellationToken cancellationToken)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();

            GetVolunteerCoordinatesResponse getVolunteerCoordinatesResponse;

            // calculating coordinates that have a minimum distance between them is expensive so cache the result
            if (request.MinDistanceBetweenInMetres != null)
            {
                Debug.WriteLine($"MinDistanceBetween not null");
                string key = $"{nameof(GetVolunteerCoordinatesResponse)}_{request}";

                getVolunteerCoordinatesResponse = await _coordinatedResetCache.GetCachedDataAsync(async () => await _getHelperCoordsByPostcodeAndRadiusGetter.GetHelperCoordsByPostcodeAndRadius(request, cancellationToken), key, CoordinatedResetCacheTime.OnHour);

            }
            else
            {
                Debug.WriteLine($"MinDistanceBetween is null");
                getVolunteerCoordinatesResponse = await _getHelperCoordsByPostcodeAndRadiusGetter.GetHelperCoordsByPostcodeAndRadius(request, cancellationToken);
            }

            sw.Stop();
            Debug.WriteLine($"GetHelperCoordsByPostcodeAndRadiusHandler took: {sw.ElapsedMilliseconds}");
            return getVolunteerCoordinatesResponse;
        }



    //public async Task<GetHelperCoordsByPostcodeAndRadiusResponse> Handle(GetHelperCoordsByPostcodeAndRadiusRequest request, CancellationToken cancellationToken)
        //{
        //    request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

        //    GetPostcodeCoordinatesRequest getPostcodeCoordsRequest = new GetPostcodeCoordinatesRequest()
        //    {
        //        Postcodes = new List<string>() { request.Postcode }
        //    };

        //    Task<GetPostcodeCoordinatesResponse> postcodeCoordsTask = _addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordsRequest, cancellationToken);
        //    Task<IEnumerable<CachedVolunteerDto>> cachedVolunteerDtosTask = _volunteerCache.GetCachedVolunteersAsync(request.VolunteerTypeEnum, request.IsVerifiedTypeEnum, cancellationToken);

        //    await Task.WhenAll(postcodeCoordsTask, cachedVolunteerDtosTask);

        //    GetPostcodeCoordinatesResponse postcodeCoords = await postcodeCoordsTask;
        //    IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await cachedVolunteerDtosTask;

        //    PostcodeCoordinate postcodeCoordsToCompareTo = postcodeCoords.PostcodeCoordinates.FirstOrDefault();

        //    List<VolunteerCoordinate> volunteerCoordinates = new List<VolunteerCoordinate>();

        //    foreach (CachedVolunteerDto cachedVolunteerDto in cachedVolunteerDtos)
        //    {
        //        double distance = _distanceCalculator.GetDistanceInMetres(postcodeCoordsToCompareTo.Latitude, postcodeCoordsToCompareTo.Longitude, cachedVolunteerDto.Latitude, cachedVolunteerDto.Longitude);

        //        bool isWithinSupportRadius = distance <= request.RadiusInMetres;

        //        if (isWithinSupportRadius)
        //        {
        //            VolunteerCoordinate volunteerCoordinate = new VolunteerCoordinate()
        //            {
        //                Latitude = cachedVolunteerDto.Latitude,
        //                Longitude = cachedVolunteerDto.Longitude,
        //                IsVerified = cachedVolunteerDto.IsVerifiedType == IsVerifiedType.IsVerified,
        //                VolunteerType = cachedVolunteerDto.VolunteerType
        //            };

        //            volunteerCoordinates.Add(volunteerCoordinate);
        //        }
        //    }
        //    GetHelperCoordsByPostcodeAndRadiusResponse getHelperCoordsByPostcodeAndRadiusResponse = new GetHelperCoordsByPostcodeAndRadiusResponse()
        //    {
        //        Coordinates = volunteerCoordinates
        //    };

        //    return getHelperCoordsByPostcodeAndRadiusResponse;
        //}
    }
}
