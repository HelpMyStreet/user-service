using AddressService.Core.Contracts;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Contracts;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;

namespace UserService.Handlers
{
    public class GetHelpersByPostcodeHandler : IRequestHandler<GetHelpersByPostcodeRequest, GetHelpersByPostcodeResponse>
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly IAddressService _addressService;
        private readonly IRepository _repository;

        public GetHelpersByPostcodeHandler(IVolunteerCache volunteerCache, IDistanceCalculator distanceCalculator, IAddressService addressService, IRepository repository)
        {
            _volunteerCache = volunteerCache;
            _distanceCalculator = distanceCalculator;
            _addressService = addressService;
            _repository = repository;
        }

        public async Task<GetHelpersByPostcodeResponse> Handle(GetHelpersByPostcodeRequest request, CancellationToken cancellationToken)
        {
            request.PostCode = PostcodeFormatter.FormatPostcode(request.PostCode);

            GetPostcodeCoordinatesRequest getPostcodeCoordsRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { request.PostCode }
            };

            Task<GetPostcodeCoordinatesResponse> postcodeCoordsTask = _addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordsRequest, cancellationToken);

            VolunteerType volunteerType = VolunteerType.Helper;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified;
            Task<IEnumerable<CachedVolunteerDto>> volunteersFromCacheTask = _volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, cancellationToken);

            await Task.WhenAll(postcodeCoordsTask, volunteersFromCacheTask);

            GetPostcodeCoordinatesResponse postcodeCoords = await postcodeCoordsTask;
            PostcodeCoordinate postcodeCoordsToCompareTo = postcodeCoords.PostcodeCoordinates.FirstOrDefault();
            IEnumerable<CachedVolunteerDto> volunteersFromCache = await volunteersFromCacheTask;

            List<int> idsOfHelpersWithinRadius = new List<int>();

            foreach (CachedVolunteerDto volunteerFromCache in volunteersFromCache)
            {
                double distance = _distanceCalculator.GetDistanceInMiles(postcodeCoordsToCompareTo.Latitude, postcodeCoordsToCompareTo.Longitude, volunteerFromCache.Latitude, volunteerFromCache.Longitude);

                bool isWithinSupportRadius = volunteerFromCache.SupportRadiusMiles <= distance;

                if (isWithinSupportRadius)
                {
                    idsOfHelpersWithinRadius.Add(volunteerFromCache.UserId);
                }
            }

            IEnumerable<User> users = await _repository.GetVolunteersByIdsAsync(idsOfHelpersWithinRadius);

            GetHelpersByPostcodeResponse response = new GetHelpersByPostcodeResponse()
            {
                Users = users.ToList()
            };

            return response;
        }
    }
}
