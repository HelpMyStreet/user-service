using HelpMyStreet.Utils.CoordinatedResetCache;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.BusinessLogic;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Extensions;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetVolunteerCoordinatesHandlerTests
    {

        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<ICoordinatedResetCache> _coordinatedResetCache;
        private Mock<IVolunteersFilteredByMinDistanceGetter> _volunteersFilteredByMinDistanceGetter;

        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtos;
        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtosReturnedByMinDistanceFilter;

        [SetUp]
        public void SetUp()
        {
            _cachedVolunteerDtos = new List<CachedVolunteerDto>()
            {
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AA",
                    Latitude = 1,
                    Longitude = 2,
                    VolunteerType = VolunteerType.Helper
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AA",
                    Latitude = 1,
                    Longitude = 2,
                    VolunteerType = VolunteerType.Helper
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AA",
                    Latitude = 1,
                    Longitude = 2,
                    VolunteerType = VolunteerType.StreetChampion
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AB",
                    Latitude = 11,
                    Longitude = 11
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AC",
                    Latitude = 1,
                    Longitude = 2,
                    VolunteerType = VolunteerType.StreetChampion
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AC",
                    Latitude = 1,
                    Longitude = 2,
                    VolunteerType = VolunteerType.StreetChampion
                },
            };

            _cachedVolunteerDtosReturnedByMinDistanceFilter = new List<CachedVolunteerDto>()
            {
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AA",
                    Latitude = 1,
                    Longitude = 2,
                    VolunteerType = VolunteerType.Helper
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AB",
                    Latitude = 11,
                    Longitude = 11
                },
            };

            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);

            _volunteersFilteredByMinDistanceGetter = new Mock<IVolunteersFilteredByMinDistanceGetter>();

            _volunteersFilteredByMinDistanceGetter.Setup(x => x.GetVolunteersFilteredByMinDistanceAsync(It.IsAny<GetVolunteerCoordinatesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtosReturnedByMinDistanceFilter);


            _coordinatedResetCache = new Mock<ICoordinatedResetCache>();

            _coordinatedResetCache.Setup(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<IEnumerable<CachedVolunteerDto>>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoordinatedResetCacheTime>())).Returns((Func<CancellationToken, Task<IEnumerable<CachedVolunteerDto>>> func, string key, CancellationToken token, CoordinatedResetCacheTime resetTime) =>
            {
                return _volunteersFilteredByMinDistanceGetter.Object.GetVolunteersFilteredByMinDistanceAsync(It.Is<GetVolunteerCoordinatesRequest>(y => y.MinDistanceBetweenInMetres == 2000), It.IsAny<CancellationToken>());
            });

        }


        [Test]
        public async Task MinDistanceIs0SoDontUseCache()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                MinDistanceBetweenInMetres = 0,
                VolunteerType = 3,
                IsVerifiedType = 3,
                SWLatitude = 1,
                SWLongitude = 2,
                NELatitude = 3,
                NELongitude = 4
            };

            GetVolunteerCoordinatesHandler getVolunteerCoordinatesHandler = new GetVolunteerCoordinatesHandler(_coordinatedResetCache.Object, _volunteerCache.Object, _volunteersFilteredByMinDistanceGetter.Object);

            GetVolunteerCoordinatesResponse result = await getVolunteerCoordinatesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(2, result.Coordinates.Count);
            Assert.AreEqual(1, result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").Latitude);
            Assert.AreEqual(2, result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").Longitude);
            Assert.AreEqual(2, result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").NumberOfHelpers);
            Assert.AreEqual(1, result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").NumberOfStreetChampions);

            Assert.AreEqual(2, result.NumberOfHelpers);
            Assert.AreEqual(3, result.NumberOfStreetChampions);
            Assert.AreEqual(5, result.TotalNumberOfVolunteers);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>()), Times.Once);


            _coordinatedResetCache.Verify(x => x.GetCachedData(It.IsAny<Func<IEnumerable<CachedVolunteerDto>>>(), It.IsAny<string>(), It.IsAny<CoordinatedResetCacheTime>()), Times.Never);

            _volunteersFilteredByMinDistanceGetter.Verify(x => x.GetVolunteersFilteredByMinDistanceAsync(It.IsAny<GetVolunteerCoordinatesRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task MinDistanceIsNot0SoUseCache()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                MinDistanceBetweenInMetres = 1,
                VolunteerType = 3,
                IsVerifiedType = 3,
                SWLatitude = 1,
                SWLongitude = 2,
                NELatitude = 3,
                NELongitude = 4
            };

            string key = $"{nameof(CachedVolunteerDto)}_MinDistance_{request.MinDistanceBetweenInMetres.RoundUpToNearest(2000)}_{request.VolunteerType}_{request.IsVerifiedType}";

            GetVolunteerCoordinatesHandler getVolunteerCoordinatesHandler = new GetVolunteerCoordinatesHandler(_coordinatedResetCache.Object, _volunteerCache.Object, _volunteersFilteredByMinDistanceGetter.Object);

            GetVolunteerCoordinatesResponse result = await getVolunteerCoordinatesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Coordinates.Count);
            Assert.AreEqual(1, result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").Latitude);
            Assert.AreEqual(2, result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").Longitude);
            Assert.IsNull(result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").NumberOfHelpers);
            Assert.IsNull(result.Coordinates.FirstOrDefault(x => x.Postcode == "NG1 1AA").NumberOfStreetChampions);

            // will need to be changed when grid aggregation functionality is implemented
            Assert.AreEqual(0, result.NumberOfHelpers);
            Assert.AreEqual(0, result.NumberOfStreetChampions);
            Assert.AreEqual(0, result.TotalNumberOfVolunteers);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>()), Times.Never);

            _coordinatedResetCache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<IEnumerable<CachedVolunteerDto>>>>(), It.Is<string>(y => y == key), It.IsAny<CancellationToken>(), It.IsAny<CoordinatedResetCacheTime>()), Times.Once);

            _volunteersFilteredByMinDistanceGetter.Verify(x => x.GetVolunteersFilteredByMinDistanceAsync(It.IsAny<GetVolunteerCoordinatesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
