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
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using UserService.Core.Interfaces.Utils;
using UserService.Core.Utils;

namespace UserService.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public AddressService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<IsPostcodeWithinRadiiResponse> IsPostcodeWithinRadiiAsync(IsPostcodeWithinRadiiRequest isPostcodeWithinRadiiRequest, CancellationToken cancellationToken)
        {
            string path = $"api/IsPostcodeWithinRadii";

            var streamContent = HttpContentUtils.SerialiseToJsonAndCompress(isPostcodeWithinRadiiRequest);

            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> isPostcodeWithinRadiiResponseWithWrapper;
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.AddressService, path, streamContent, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                isPostcodeWithinRadiiResponseWithWrapper = await Utf8Json.JsonSerializer.DeserializeAsync<ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>>(stream);
            }

            if (!isPostcodeWithinRadiiResponseWithWrapper.IsSuccessful)
            {
                throw new Exception($"Calling Address Service IsPostcodeWithinRadii endpoint unsuccessful: {isPostcodeWithinRadiiResponseWithWrapper.Errors.FirstOrDefault()?.ErrorMessage}");
            }

            return isPostcodeWithinRadiiResponseWithWrapper.Content;
        }
    }
}
