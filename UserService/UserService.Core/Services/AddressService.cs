using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddressService.Core.Contracts;
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using UserService.Core.Interfaces.Utils;
using UserService.Core.Utils;
using Utf8Json.Resolvers;

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
            var sw = new Stopwatch();
            sw.Start();
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.AddressService, path, streamContent, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                sw.Stop();
                Debug.WriteLine($"Calling Address Service GetPostcodeCoordinatesAsync took: {sw.ElapsedMilliseconds}");
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
