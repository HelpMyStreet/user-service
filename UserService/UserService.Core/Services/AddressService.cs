﻿using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using UserService.Core.Interfaces.Services;
using Utf8Json.Resolvers;
using HelpMyStreet.Utils.Enums;

namespace UserService.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public AddressService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<GetPostcodeCoordinatesResponse> GetPostcodeCoordinatesAsync(GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest, CancellationToken cancellationToken)
        {
            string path = $"api/GetPostcodeCoordinates";

            var streamContent = HttpContentUtils.SerialiseToJsonAndCompress(getPostcodeCoordinatesRequest);

            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> getPostcodeCoordinatesResponseWithWrapper;
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.AddressService, path, streamContent, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                getPostcodeCoordinatesResponseWithWrapper = await Utf8Json.JsonSerializer.DeserializeAsync<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>>(stream, StandardResolver.AllowPrivate);
            }

            if (!getPostcodeCoordinatesResponseWithWrapper.IsSuccessful)
            {
                throw new Exception($"Calling Address Service GetPostcodeCoordinatesAsync endpoint unsuccessful: {getPostcodeCoordinatesResponseWithWrapper.Errors.FirstOrDefault()?.ErrorMessage}");
            }

            return getPostcodeCoordinatesResponseWithWrapper.Content;
        }
    }
}
