using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.UnitTests
{
    public class AddressServiceTests
    {
        private Mock<IHttpClientWrapper> _httpClientWrapper;

        private GetPostcodeCoordinatesResponse _getPostcodeCoordinatesResponse;

        [SetUp]
        public void SetUp()
        {
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.SetupAllProperties();

            _getPostcodeCoordinatesResponse = new GetPostcodeCoordinatesResponse()
            {
               PostcodeCoordinates = new List<PostcodeCoordinate>()
               {
                   new PostcodeCoordinate()
                   {
                       Postcode = "NG1 1AA",
                       Latitude = 45,
                       Longitude = 50
                   }
               }
            };

            HttpResponseMessage httpResponseMessage = CreateSuccessfulResponseWrappersHttpResponseMessage(_getPostcodeCoordinatesResponse);
            _httpClientWrapper.Setup(x => x.PostAsync(It.IsAny<HttpClientConfigName>(), It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>())).ReturnsAsync(httpResponseMessage);

        }


        private HttpResponseMessage CreateSuccessfulResponseWrappersHttpResponseMessage<T>(T content)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(ResponseWrapper<T, AddressServiceErrorCode>.CreateSuccessfulResponse(content)))
            };

            return httpResponseMessage;
        }

        private HttpResponseMessage CreateUnsuccesssfuResponseWrapperHttpResponseMessage<T>()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(ResponseWrapper<T, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Error")))
            };

            return httpResponseMessage;
        }

        [Test]
        public async Task Success()
        {
            GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>()
                {
                    "NG1 1AA"
                },
                
            };

            Core.Services.AddressService addressService = new Core.Services.AddressService(_httpClientWrapper.Object);

            GetPostcodeCoordinatesResponse result = await addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordinatesRequest, CancellationToken.None);

            Assert.IsTrue(result.PostcodeCoordinates.Any(x=>x.Postcode == "NG1 1AA" && x.Latitude == 45 && x.Longitude == 50));

            _httpClientWrapper.Setup(x => x.PostAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.AddressService), It.Is<string>(y => y == "api/GetPostcodeCoordinates"), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Unsuccessful()
        {
            HttpResponseMessage httpResponseMessage = CreateUnsuccesssfuResponseWrapperHttpResponseMessage<GetPostcodeCoordinatesResponse>();
            _httpClientWrapper.Setup(x => x.PostAsync(It.IsAny<HttpClientConfigName>(), It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>())).ReturnsAsync(httpResponseMessage);

            GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>()
                {
                    "NG1 1AA"
                },
            };

            Core.Services.AddressService addressService = new Core.Services.AddressService(_httpClientWrapper.Object);

            Exception ex = Assert.ThrowsAsync<Exception>(async () => await addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordinatesRequest, CancellationToken.None));

            Assert.AreEqual("Calling Address Service GetPostcodeCoordinatesAsync endpoint unsuccessful: Error", ex.Message);
        }

    }
}
