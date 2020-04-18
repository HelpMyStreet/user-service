using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using UserService.Core.Interfaces.Utils;

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

            byte[] serialisedBytes = Utf8Json.JsonSerializer.Serialize(isPostcodeWithinRadiiRequest);

            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(serialisedBytes, 0, serialisedBytes.Length);
            }

            memoryStream.Position = 0;
            StreamContent streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            streamContent.Headers.ContentEncoding.Add("gzip");

            ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode> isPostcodeWithinRadiiResponseWithWrapper;
            var sw = new Stopwatch();
            sw.Start();
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.AddressService, path, streamContent, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                Stream stream = await response.Content.ReadAsStreamAsync();
                sw.Stop();
                Debug.WriteLine($"# Calling Address Service took { sw.ElapsedMilliseconds}");
                isPostcodeWithinRadiiResponseWithWrapper = await Utf8Json.JsonSerializer.DeserializeAsync<ResponseWrapper<IsPostcodeWithinRadiiResponse, AddressServiceErrorCode>>(stream);
            }

            if (!isPostcodeWithinRadiiResponseWithWrapper.IsSuccessful)
            {
                throw new Exception($"Calling Address Service IsPostcodeWithinRadii endpoint unsucessful: {isPostcodeWithinRadiiResponseWithWrapper.Errors.FirstOrDefault()?.ErrorMessage}");
            }


            return isPostcodeWithinRadiiResponseWithWrapper.Content;
        }
    }
}
