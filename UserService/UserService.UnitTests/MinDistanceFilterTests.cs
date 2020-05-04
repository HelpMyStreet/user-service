using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UserService.Core.BusinessLogic;
using UserService.Core.Dto;
using UserService.Core.Utils;

namespace UserService.UnitTests
{
    public class MinDistanceFilterTests
    {
        private IEnumerable<ILatitudeLongitude> _cachedVolunteerDtos;
        private Mock<IDistanceCalculator> _distanceCalculator;
        
        [SetUp]
        public void SetUp()
        {
            ILatitudeLongitude a = Mock.Of<ILatitudeLongitude>(x => x.Latitude == 1 && x.Longitude == 2d);

            _cachedVolunteerDtos = new List<ILatitudeLongitude>()
            {
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 1d && x.Longitude == 2d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 3d && x.Longitude == 4d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 5d && x.Longitude == 6d),
            };

            _distanceCalculator = new Mock<IDistanceCalculator>();

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 1), It.Is<double>(y => y == 2))).Returns(2);

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 3), It.Is<double>(y => y == 4))).Returns(2.1);

            _distanceCalculator.Setup(x => x.GetDistanceInMetres(It.IsAny<double>(), It.IsAny<double>(), It.Is<double>(y => y == 5), It.Is<double>(y => y == 6))).Returns(10);

        }

        [Test]
        public void FilterByMinDistance_MinDistance0()
        {
            MinDistanceFilter minDistanceFilter = new MinDistanceFilter(_distanceCalculator.Object);

            IEnumerable<ILatitudeLongitude> result = minDistanceFilter.FilterByMinDistance(_cachedVolunteerDtos, 0);

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(x => x.Latitude == 1 && x.Longitude == 2));
            Assert.IsTrue(result.Any(x => x.Latitude == 3 && x.Longitude == 4));
            Assert.IsTrue(result.Any(x => x.Latitude == 5 && x.Longitude == 6));
        }

        [Test]
        public void FilterByMinDistance_MinDistance10()
        {
            MinDistanceFilter minDistanceFilter = new MinDistanceFilter(_distanceCalculator.Object);

            IEnumerable<ILatitudeLongitude> result = minDistanceFilter.FilterByMinDistance(_cachedVolunteerDtos, 10);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.Latitude == 1 && x.Longitude == 2));
            Assert.IsTrue(result.Any(x => x.Latitude == 5 && x.Longitude == 6));
        }

        [Test]
        public void FilterByMinDistance_MinDistance100()
        {
            MinDistanceFilter minDistanceFilter = new MinDistanceFilter(_distanceCalculator.Object);

            IEnumerable<ILatitudeLongitude> result = minDistanceFilter.FilterByMinDistance(_cachedVolunteerDtos, 100);

            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(x => x.Latitude == 1 && x.Longitude == 2));
        }
    }
}
