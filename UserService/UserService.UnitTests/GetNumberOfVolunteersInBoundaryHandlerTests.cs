using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetNumberOfVolunteersInBoundaryHandlerTests
    {
        private Mock<IVolunteerCache> _volunteerCache;

        private IEnumerable<CachedVolunteerDto> _cachedHelperDtos;
        private IEnumerable<CachedVolunteerDto> _cachedStreetChampionDtos;

        [SetUp]
        public void SetUp()
        {
            _cachedHelperDtos = new List<CachedVolunteerDto>()
            {
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AA",
                    Latitude = 1,
                    Longitude = 1,
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AB",
                    Latitude = 1,
                    Longitude = 1
                },
                new CachedVolunteerDto()
                {
                Postcode = "NG1 1AC",
                Latitude = 1,
                Longitude = 1
            },
            };

            _cachedStreetChampionDtos = new List<CachedVolunteerDto>()
            {
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AC",
                    Latitude = 1,
                    Longitude = 1,
                },
                new CachedVolunteerDto()
                {
                    Postcode = "NG1 1AD",
                    Latitude = 1,
                    Longitude = 1
                },
            };

            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == VolunteerType.StreetChampion), It.Is<IsVerifiedType>(y => (int)y == 3), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedStreetChampionDtos);

            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == VolunteerType.Helper), It.Is<IsVerifiedType>(y => (int)y == 3), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedHelperDtos);

        }


        [Test]
        public async Task GetVolunteerCounts()
        {
            GetNumberOfVolunteersInBoundaryRequest request = new GetNumberOfVolunteersInBoundaryRequest()
            {
                SwLatitude = 0,
                SwLongitude = 0,
                NeLatitude = 2,
                NeLongitude = 2
            };


            GetNumberOfVolunteersInBoundaryHandler getNumberOfVolunteersInBoundaryHandler = new GetNumberOfVolunteersInBoundaryHandler(_volunteerCache.Object);

            GetNumberOfVolunteersInBoundaryResponse result = await getNumberOfVolunteersInBoundaryHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(2, result.NumberOfStreetChampions);
            Assert.AreEqual(3, result.NumberOfHelpers);
            Assert.AreEqual(5, result.TotalNumberOfVolunteers);
        }
    }
}
