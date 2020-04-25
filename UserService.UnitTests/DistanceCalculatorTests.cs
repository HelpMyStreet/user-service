using NuGet.Frameworks;
using NUnit.Framework;
using UserService.Core.Utils;

namespace UserService.UnitTests
{
    public class DistanceCalculatorTests
    {
        // latitudes, longitudes and distance taken from postcode IO
        [TestCase(52.954885, -1.155263, 52.955491, -1.155413, 68.18827704)]
        [TestCase(54.286959, -0.393904, 54.284261, -0.38845, 465.13933511)]
        [TestCase(53.402961, -2.995571, 53.406163, -2.991088, 464.64088409)]
        [TestCase(53.388313, -1.472253, 53.389387, -1.477352, 359.69046967)]
        public void GetDistanceInMetres(double longitude, double latitude, double otherLongitude, double otherLatitude, double expectedDistanceInMetres)
        {
          DistanceCalculator distanceCalculator = new DistanceCalculator();
          double result = distanceCalculator.GetDistanceInMetres(longitude, latitude, otherLongitude, otherLatitude);

          Assert.IsTrue(result >= (expectedDistanceInMetres - 1) && result <= (expectedDistanceInMetres + 1));
        }

        [TestCase(52.954885, -1.155263, 52.955491, -1.155413, 0.042370231000954)]
        [TestCase(54.286959, -0.393904, 54.284261, -0.38845, 0.289024183213781)]
        [TestCase(53.402961, -2.995571, 53.406163, -2.991088, 0.288714460109212)]
        [TestCase(53.388313, -1.472253, 53.389387, -1.477352, 0.223501295975255)]
        public void GetDistanceInMiles(double longitude, double latitude, double otherLongitude, double otherLatitude, double expectedDistanceInMetres)
        {
            DistanceCalculator distanceCalculator = new DistanceCalculator();
            double result = distanceCalculator.GetDistanceInMiles(longitude, latitude, otherLongitude, otherLatitude);

            Assert.IsTrue(result >= (expectedDistanceInMetres - 0.01) && result <= (expectedDistanceInMetres + 0.01));
        }
    }
}
