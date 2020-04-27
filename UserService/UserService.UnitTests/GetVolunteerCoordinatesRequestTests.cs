using NUnit.Framework;
using UserService.Core.Domains.Entities;

namespace UserService.UnitTests
{
    public class GetVolunteerCoordinatesRequestTests
    {
        [Test(Description = "The ToString() logic is important because it's used as a cache key")]
        public void TestToString()
        {
            GetVolunteerCoordinatesRequest getVolunteerCoordinatesRequest = new GetVolunteerCoordinatesRequest()
            {
                Latitude = 1,
                Longitude = 2,
                VolunteerType = 3,
                IsVerifiedType = 2,
                RadiusInMetres = 1,
                MinDistanceBetweenInMetres = 2
            };

            string result = getVolunteerCoordinatesRequest.ToString();

            Assert.AreEqual("Latitude: 1, Longitude: 2, RadiusInMetres: 1, VolunteerType: 3,  IsVerifiedType: 2, MinDistanceBetweenInMetres: 2", result);
        }
    }
}
