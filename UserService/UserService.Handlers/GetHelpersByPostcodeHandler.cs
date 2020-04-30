using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using UserService.Core;
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
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            GetPostcodeCoordinatesRequest getPostcodeCoordsRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { request.Postcode }
            };

            Task<GetPostcodeCoordinatesResponse> postcodeCoordsTask = _addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordsRequest, cancellationToken);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified;
            Task<IEnumerable<CachedVolunteerDto>> cachedVolunteerDtosTask = _volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, cancellationToken);

            await Task.WhenAll(postcodeCoordsTask, cachedVolunteerDtosTask);

            GetPostcodeCoordinatesResponse postcodeCoords = await postcodeCoordsTask;
            PostcodeCoordinate postcodeCoordsToCompareTo = postcodeCoords.PostcodeCoordinates.FirstOrDefault();
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await cachedVolunteerDtosTask;

            List<int> idsOfHelpersWithinRadius = new List<int>();

            foreach (CachedVolunteerDto cachedVolunteerDto in cachedVolunteerDtos)
            {
                double distance = _distanceCalculator.GetDistanceInMiles(postcodeCoordsToCompareTo.Latitude, postcodeCoordsToCompareTo.Longitude, cachedVolunteerDto.Latitude, cachedVolunteerDto.Longitude);

                bool isWithinSupportRadius = distance <= cachedVolunteerDto.SupportRadiusMiles;

                if (isWithinSupportRadius)
                {
                    idsOfHelpersWithinRadius.Add(cachedVolunteerDto.UserId);
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
