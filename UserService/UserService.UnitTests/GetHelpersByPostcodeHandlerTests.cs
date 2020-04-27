using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetHelpersByPostcodeHandlerTests
    {
        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<IDistanceCalculator> _distanceCalculator;
        private Mock<IAddressService> _addressService;
        private Mock<IRepository> _repository;

        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtos;

        private GetPostcodeCoordinatesResponse _getPostcodeCoordinatesResponse;

        private IEnumerable<User> _users;

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
                    SupportRadiusMiles = 2
                },
                new CachedVolunteerDto()
                {
                    UserId = 2,
                    Postcode = "NG1 1AB",
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsVerified,
                    Latitude = 3,
                    Longitude = 4,
                    SupportRadiusMiles = 1.9
                },
            };


            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);

            _distanceCalculator = new Mock<IDistanceCalculator>();

            _distanceCalculator.Setup(x => x.GetDistanceInMiles(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(2);


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


            _users = new List<User>()
            {
                new User()
                {
                    ID = 1
                }
            };
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetVolunteersByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(_users);
        }

        [Test]
        public async Task GetAllVolunteersWithinRadius()
        {

            GetHelpersByPostcodeRequest request = new GetHelpersByPostcodeRequest()
            {
                Postcode = "NG1 1AE"
            };

            GetHelpersByPostcodeHandler getHelpersByPostcodeHandler = new GetHelpersByPostcodeHandler(_volunteerCache.Object, _distanceCalculator.Object, _addressService.Object, _repository.Object);

            GetHelpersByPostcodeResponse result = await getHelpersByPostcodeHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Users.Count);
            Assert.AreEqual(1, result.Users.FirstOrDefault().ID);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == (VolunteerType.Helper | VolunteerType.StreetChampion)), It.Is<IsVerifiedType>(y => y == IsVerifiedType.IsVerified), It.IsAny<CancellationToken>()));

            _repository.Verify(x => x.GetVolunteersByIdsAsync(It.Is<IEnumerable<int>>(y => y.Count() == 1 && y.Any(z => z == 1))));

            _addressService.Verify(x => x.GetPostcodeCoordinatesAsync(It.Is<GetPostcodeCoordinatesRequest>(y => y.Postcodes.Count() == 1 && y.Postcodes.Contains("NG1 1AE")), It.IsAny<CancellationToken>()));
        }
    }
}
