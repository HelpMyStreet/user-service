//using HelpMyStreet.Contracts.AddressService.Request;
//using HelpMyStreet.Contracts.AddressService.Response;
//using HelpMyStreet.Contracts.Shared;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using UserService.Core.Config;
//using UserService.Core.Interfaces.Utils;
//using UserService.Core.Services;

//namespace UserService.UnitTests
//{
//    public class AddressServiceTests
//    {
//        private Mock<IHttpClientWrapper> _httpClientWrapper;

//        private IsPostcodeWithinRadiiResponse _isPostcodeWithinRadiiResponse;

//        [SetUp]
//        public void SetUp()
//        {
//            _httpClientWrapper = new Mock<IHttpClientWrapper>();
//            _httpClientWrapper.SetupAllProperties();

//            _isPostcodeWithinRadiiResponse = new IsPostcodeWithinRadiiResponse()
//            {
//                IdsWithinRadius = new List<int>() { 1 }
//            };

//            HttpResponseMessage httpResponseMessage = CreateSuccessfulResponseWrappersHttpResponseMessage(_isPostcodeWithinRadiiResponse);
//            _httpClientWrapper.Setup(x => x.PostAsync(It.IsAny<HttpClientConfigName>(), It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>())).ReturnsAsync(httpResponseMessage);

//        }


//        private HttpResponseMessage CreateSuccessfulResponseWrappersHttpResponseMessage<T>(T content)
//        {
//            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
//            {
//                Content = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(ResponseWrapper<T, AddressServiceErrorCode>.CreateSuccessfulResponse(content)))
//            };

//            return httpResponseMessage;
//        }

//        private HttpResponseMessage CreateUnsuccesssfuResponseWrapperHttpResponseMessage<T>()
//        {
//            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
//            {
//                Content = new ByteArrayContent(Utf8Json.JsonSerializer.Serialize(ResponseWrapper<T, AddressServiceErrorCode>.CreateUnsuccessfulResponse(AddressServiceErrorCode.UnhandledError, "Error")))
//            };

//            return httpResponseMessage;
//        }

//        [Test]
//        public async Task Success()
//        {
//            IsPostcodeWithinRadiiRequest isPostcodeWithinRadiiRequest = new IsPostcodeWithinRadiiRequest()
//            {
//                Postcode = "NG1 5FS",
//                PostcodeWithRadiuses = new List<PostcodeWithRadius>()
//            };

//            Core.Services.AddressService addressService = new Core.Services.AddressService(_httpClientWrapper.Object);

//            IsPostcodeWithinRadiiResponse result = await addressService.IsPostcodeWithinRadiiAsync(isPostcodeWithinRadiiRequest, CancellationToken.None);

//            Assert.IsTrue(result.IdsWithinRadius.Contains(1));

//            _httpClientWrapper.Setup(x => x.PostAsync(It.Is<HttpClientConfigName>(y => y == HttpClientConfigName.AddressService), It.Is<string>(y => y == "api/IsPostcodeWithinRadii"), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()));
//        }

//        [Test]
//        public async Task Unsuccessful()
//        {
//            HttpResponseMessage httpResponseMessage = CreateUnsuccesssfuResponseWrapperHttpResponseMessage<IsPostcodeWithinRadiiResponse>();
//            _httpClientWrapper.Setup(x => x.PostAsync(It.IsAny<HttpClientConfigName>(), It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>())).ReturnsAsync(httpResponseMessage);

//            IsPostcodeWithinRadiiRequest isPostcodeWithinRadiiRequest = new IsPostcodeWithinRadiiRequest()
//            {
//                Postcode = "NG1 5FS",
//                PostcodeWithRadiuses = new List<PostcodeWithRadius>()
//            };

//            AddressService addressService = new AddressService(_httpClientWrapper.Object);

//            Exception ex = Assert.ThrowsAsync<Exception>(async () => await addressService.IsPostcodeWithinRadiiAsync(isPostcodeWithinRadiiRequest, CancellationToken.None));

//            Assert.AreEqual("Calling Address Service IsPostcodeWithinRadii endpoint unsuccessful: Error", ex.Message);
//        }

//    }
//}
