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
using UserService.Core.PreCalculation;

namespace UserService.UnitTests
{
    public class VolunteersFilteredByMinDistanceGetterTests
    {

        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<IMinDistanceFilter> _minDistanceFilter;

        private IEnumerable<PrecalculatedVolunteerDto> _cachedVolunteerDtos;

        [SetUp]
        public void SetUp()
        {
            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.SetupAllProperties();

            _cachedVolunteerDtos = new List<PrecalculatedVolunteerDto>()
            {
                new PrecalculatedVolunteerDto()
                {
                    UserId = 1,
                    Latitude = 2,
                    Longitude = 3
                }
            };
            _minDistanceFilter = new Mock<IMinDistanceFilter>();
            _minDistanceFilter.Setup(x => x.FilterByMinDistance(It.IsAny<IEnumerable<PrecalculatedVolunteerDto>>(), It.IsAny<int>())).Returns(_cachedVolunteerDtos);
        }

        [Test]
        public async Task GetVolunteersFilteredByMinDistanceAsync()
        {
            GetVolunteerCoordinatesRequest request = new GetVolunteerCoordinatesRequest()
            {
                VolunteerType = 3,
                IsVerifiedType = 3,
                MinDistanceBetweenInMetres = 100
            };

            VolunteersFilteredByMinDistanceGetter volunteersFilteredByMinDistanceGetter = new VolunteersFilteredByMinDistanceGetter(_volunteerCache.Object, _minDistanceFilter.Object);

            IEnumerable<PrecalculatedVolunteerDto> result = await volunteersFilteredByMinDistanceGetter.GetVolunteersFilteredByMinDistanceAsync(request, CancellationToken.None);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.FirstOrDefault().UserId);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => (int)y == 3), It.Is<IsVerifiedType>(y => (int)y == 3), It.IsAny<CancellationToken>()));

            _minDistanceFilter.Verify(x => x.FilterByMinDistance(It.IsAny<IEnumerable<PrecalculatedVolunteerDto>>(), It.Is<int>(y => y == 100)));

        }


    }
}
