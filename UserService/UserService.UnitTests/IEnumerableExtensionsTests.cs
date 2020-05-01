using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UserService.Core.Dto;
using UserService.Core.Extensions;

namespace UserService.UnitTests
{
    public class IEnumerableExtensionsTests
    {
        [Test]
        public void WhereWithinBoundary()
        {
            List<ILatitudeLongitude> latLngs = new List<ILatitudeLongitude>()
            {
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 0d && x.Longitude == 0d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 5d && x.Longitude == 5d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10d && x.Longitude == 10d),

                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10d && x.Longitude == 10.1d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10.1d && x.Longitude == 10d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 10.1d && x.Longitude == 10.1d),

                Mock.Of<ILatitudeLongitude>(x => x.Latitude == -0.1d && x.Longitude == 0d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == 0d && x.Longitude == -0.1d),
                Mock.Of<ILatitudeLongitude>(x => x.Latitude == -0.1d && x.Longitude == -0.1d),
            };

            IEnumerable<ILatitudeLongitude> result = latLngs.WhereWithinBoundary(0, 0, 10, 10);

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(x => x.Latitude == 0d && x.Longitude == 0d));
            Assert.IsTrue(result.Any(x => x.Latitude == 5d && x.Longitude == 5d));
            Assert.IsTrue(result.Any(x => x.Latitude == 10d && x.Longitude == 10d));
        }
    }
}
