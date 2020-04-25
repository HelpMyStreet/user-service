using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetHelperCoordsByPostcodeAndRadiusHandlerTests
    {
        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<IDistanceCalculator> _distanceCalculator;
        private Mock<IAddressService> _addressService;

        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtos;

        private GetPostcodeCoordinatesResponse _getPostcodeCoordinatesResponse;

        [SetUp]
        public void SetUp()
        {
            _cachedVolunteerDtos = new List<CachedVolunteerDto>()
            {
                new CachedVolunteerDto()
                {
                    UserId = 1,
                    Postcode = "NG1 1AA",
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsVerified,
                    Latitude = 1,
                    Longitude = 2,
                    SupportRadiusMiles = 0
                },
                new CachedVolunteerDto()
                {
                    UserId = 2,
                    Postcode = "NG1 1AB",
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsVerified,
                    Latitude = 3,
                    Longitude = 4,
                    SupportRadiusMiles = 0
                },
            };


            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);

            _distanceCalculator = new Mock<IDistanceCalculator>();

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 1), It.Is<double>(y => y == 2))).Returns(2);

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 3), It.Is<double>(y => y == 4))).Returns(2.1);


            _getPostcodeCoordinatesResponse = new GetPostcodeCoordinatesResponse()
            {
                PostcodeCoordinates = new List<PostcodeCoordinate>()
                {
                    new PostcodeCoordinate()
                    {
                        Postcode = "NG1 1AE",
                        Latitude = 9,
                        Longitude = 10
                    }
                }
            };

            _addressService = new Mock<IAddressService>();

            _addressService.Setup(x => x.GetPostcodeCoordinatesAsync(It.IsAny<GetPostcodeCoordinatesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_getPostcodeCoordinatesResponse);

        }

        [Test]
        public async Task GetVolunteerCoordinates()
        {

            GetHelperCoordsByPostcodeAndRadiusRequest request = new GetHelperCoordsByPostcodeAndRadiusRequest()
            {
                Postcode = "NG1 1AE",
                IsVerifiedType = 3,
                VolunteerType = 3,
                RadiusInMetres = 2
            };

            GetHelperCoordsByPostcodeAndRadiusHandler getHelperCoordsByPostcodeAndRadiusHandler = new GetHelperCoordsByPostcodeAndRadiusHandler(_volunteerCache.Object, _distanceCalculator.Object, _addressService.Object);

            GetHelperCoordsByPostcodeAndRadiusResponse result = await getHelperCoordsByPostcodeAndRadiusHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Coordinates.Count);
            Assert.AreEqual(1, result.Coordinates.FirstOrDefault().Latitude);
            Assert.AreEqual(2, result.Coordinates.FirstOrDefault().Longitude);
            Assert.AreEqual(VolunteerType.Helper, result.Coordinates.FirstOrDefault().VolunteerType);
            Assert.AreEqual(true, result.Coordinates.FirstOrDefault().IsVerified);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == (VolunteerType.Helper | VolunteerType.StreetChampion)), It.Is<IsVerifiedType>(y => y == (IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified)), It.IsAny<CancellationToken>()));


            _addressService.Verify(x => x.GetPostcodeCoordinatesAsync(It.Is<GetPostcodeCoordinatesRequest>(y => y.Postcodes.Count() == 1 && y.Postcodes.Contains("NG1 1AE")), It.IsAny<CancellationToken>()));
        }
    }
}
