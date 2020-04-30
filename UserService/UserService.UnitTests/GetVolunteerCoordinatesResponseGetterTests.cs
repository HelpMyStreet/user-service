using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.BusinessLogic;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Utils;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetVolunteerCoordinatesResponseGetterTests
    {
        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<IDistanceCalculator> _distanceCalculator;

        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtos;

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

                new CachedVolunteerDto()
                {
                    UserId = 3,
                    Postcode = "NG1 1AC",
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsVerified,
                    Latitude = 5,
                    Longitude = 6,
                    SupportRadiusMiles = 0
                },
            };


            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);

            _distanceCalculator = new Mock<IDistanceCalculator>();

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 1), It.Is<double>(y => y == 2))).Returns(2);

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 3), It.Is<double>(y => y == 4))).Returns(2.1);

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 5), It.Is<double>(y => y == 6))).Returns(10);
        }

        [Test]
        public async Task GetVolunteerCoordinatesWithRadiusAndMinDistanceIs0()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                Latitude = 1,
                Longitude = 2,
                IsVerifiedType = 3,
                VolunteerType = 3,
                RadiusInMetres = 2,
                MinDistanceBetweenInMetres = 0
            };

            GetVolunteerCoordinatesResponseGetter getVolunteerCoordinatesResponseGetter = new GetVolunteerCoordinatesResponseGetter(_volunteerCache.Object, _distanceCalculator.Object);

            GetVolunteerCoordinatesResponse result = await getVolunteerCoordinatesResponseGetter.GetVolunteerCoordinates(request, CancellationToken.None);

            Assert.AreEqual(1, result.Coordinates.Count);
            Assert.AreEqual(1, result.Coordinates.FirstOrDefault().Latitude);
            Assert.AreEqual(2, result.Coordinates.FirstOrDefault().Longitude);
            Assert.AreEqual(VolunteerType.Helper, result.Coordinates.FirstOrDefault().VolunteerType);
            Assert.AreEqual(true, result.Coordinates.FirstOrDefault().IsVerified);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == (VolunteerType.Helper | VolunteerType.StreetChampion)), It.Is<IsVerifiedType>(y => y == (IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetVolunteerCoordinatesWith0AsRadiusAndAMinDistance()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                Latitude = 1,
                Longitude = 2,
                IsVerifiedType = 3,
                VolunteerType = 3,
                RadiusInMetres = 0,
                MinDistanceBetweenInMetres = 10
            };

            GetVolunteerCoordinatesResponseGetter getVolunteerCoordinatesResponseGetter = new GetVolunteerCoordinatesResponseGetter(_volunteerCache.Object, _distanceCalculator.Object);

            GetVolunteerCoordinatesResponse result = await getVolunteerCoordinatesResponseGetter.GetVolunteerCoordinates(request, CancellationToken.None);

            Assert.AreEqual(2, result.Coordinates.Count);
            Assert.IsTrue(result.Coordinates.Any(x => x.Latitude == 1 && x.Longitude == 2));
            Assert.IsTrue(result.Coordinates.Any(x => x.Latitude == 5 && x.Longitude == 6));

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == (VolunteerType.Helper | VolunteerType.StreetChampion)), It.Is<IsVerifiedType>(y => y == (IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified)), It.IsAny<CancellationToken>()));
        }



        [Test]
        public async Task GetVolunteerCoordinatesWith0AsRadiusAndMinDistanceIs0()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                Latitude = 1,
                Longitude = 2,
                IsVerifiedType = 3,
                VolunteerType = 3,
                RadiusInMetres = 0,
                MinDistanceBetweenInMetres = 0
            };

            GetVolunteerCoordinatesResponseGetter getVolunteerCoordinatesResponseGetter = new GetVolunteerCoordinatesResponseGetter(_volunteerCache.Object, _distanceCalculator.Object);

            GetVolunteerCoordinatesResponse result = await getVolunteerCoordinatesResponseGetter.GetVolunteerCoordinates(request, CancellationToken.None);

            Assert.AreEqual(3, result.Coordinates.Count);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == (VolunteerType.Helper | VolunteerType.StreetChampion)), It.Is<IsVerifiedType>(y => y == (IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified)), It.IsAny<CancellationToken>()));
        }
    }
}
