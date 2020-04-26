﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Cache;
using UserService.Core.Domains.Entities;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetHelperCoordsByPostcodeAndRadiusHandlerTests
    {

        private Mock<IGetHelperCoordsByPostcodeAndRadiusGetter> _getHelperCoordsByPostcodeAndRadiusGetter;
        private Mock<ICoordinatedResetCache> _coordinatedResetCache;

        private GetVolunteerCoordinatesResponse _getVolunteerCoordinatesResponse;

        [SetUp]
        public void SetUp()
        {
            _getVolunteerCoordinatesResponse = new GetVolunteerCoordinatesResponse()
            {
                Coordinates = new List<VolunteerCoordinate>()
                {
                    new VolunteerCoordinate()
                    {
                        Longitude = 1,
                        Latitude = 2,
                        IsVerified = true,
                        VolunteerType = VolunteerType.StreetChampion
                    }
                }
            };

            _getHelperCoordsByPostcodeAndRadiusGetter = new Mock<IGetHelperCoordsByPostcodeAndRadiusGetter>();

            _getHelperCoordsByPostcodeAndRadiusGetter.Setup(x => x.GetHelperCoordsByPostcodeAndRadius(It.IsAny<GetVolunteerCoordinatesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(_getVolunteerCoordinatesResponse);

            _coordinatedResetCache = new Mock<ICoordinatedResetCache>();


            _coordinatedResetCache.Setup(x => x.GetCachedDataAsync(It.IsAny<Func<Task<GetVolunteerCoordinatesResponse>>>(), It.IsAny<string>(), It.IsAny<CoordinatedResetCacheTime>())).ReturnsAsync(_getVolunteerCoordinatesResponse);

        }

        [Test]
        public async Task MinDistanceBetweenNotNullSoUseCache()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                Longitude = 1,
                Latitude = 2,
                MinDistanceBetweenInMetres = 25000,
                VolunteerType = 3,
                IsVerifiedType = 3
            };

            GetVolunteerCoordinatesHandler getVolunteerCoordinatesHandler = new GetVolunteerCoordinatesHandler(_getHelperCoordsByPostcodeAndRadiusGetter.Object, _coordinatedResetCache.Object);

            var result = await getVolunteerCoordinatesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Coordinates.Count);

            string key = $"{nameof(GetVolunteerCoordinatesResponse)}_{request}";
            _coordinatedResetCache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<Task<GetVolunteerCoordinatesResponse>>>(), It.Is<string>( y=> y== key), It.Is<CoordinatedResetCacheTime>(y => y== CoordinatedResetCacheTime.OnHour)), Times.Once);
        }

        [Test]
        public async Task MinDistanceBetweenNullSoDontUseCache()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                Longitude = 1,
                Latitude = 2,
                MinDistanceBetweenInMetres = null,
                VolunteerType = 3,
                IsVerifiedType = 3
            };

            GetVolunteerCoordinatesHandler getVolunteerCoordinatesHandler = new GetVolunteerCoordinatesHandler(_getHelperCoordsByPostcodeAndRadiusGetter.Object, _coordinatedResetCache.Object);

            var result = await getVolunteerCoordinatesHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Coordinates.Count);

            _coordinatedResetCache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<Task<GetVolunteerCoordinatesResponse>>>(), It.IsAny<string>(), It.IsAny<CoordinatedResetCacheTime>()), Times.Never);
        }

    }
}
