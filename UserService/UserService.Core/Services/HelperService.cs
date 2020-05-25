﻿using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Interfaces.Utils;
using UserService.Core.Utils;
using Utf8Json.Resolvers;

namespace UserService.Core.Services
{
    public class HelperService : IHelperService
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IDistanceCalculator _distanceCalculator;
        private readonly IAddressService _addressService;
        private readonly IRepository _repository;
        public HelperService(IAddressService addressService, IVolunteerCache volunteerCache, IDistanceCalculator distanceCalculator, IRepository repository)
        {
            _addressService = addressService;
            _volunteerCache = volunteerCache;
            _distanceCalculator = distanceCalculator;
            _repository = repository;
        }


        public async Task<IEnumerable<User>> GetHelpersWithinRadius(string postcode, CancellationToken token)
        {

            GetPostcodeCoordinatesRequest getPostcodeCoordsRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { postcode }
            };

            Task<GetPostcodeCoordinatesResponse> postcodeCoordsTask = _addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordsRequest, token);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified;
            Task<IEnumerable<CachedVolunteerDto>> cachedVolunteerDtosTask = _volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, token);

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

            return  await _repository.GetVolunteersByIdsAsync(idsOfHelpersWithinRadius);
        }
    }
}