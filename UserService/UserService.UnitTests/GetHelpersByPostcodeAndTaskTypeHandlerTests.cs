using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
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
using UserService.Core.Services;
using UserService.Core.Utils;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetVolunteersByPostcodeAndActivityHandlerTests
    {
        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<IDistanceCalculator> _distanceCalculator;
        private Mock<IAddressService> _addressService;
        private IHelperService _helperService;
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

            _repository = new Mock<IRepository>();

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
            _helperService = new HelperService(_addressService.Object, _volunteerCache.Object, _distanceCalculator.Object, _repository.Object);

            _users = new List<User>()
            {
                new User()
                {
                    ID = 1,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test",
                        EmailAddress = "test@test.com"
                    },
                    SupportActivities = new List<HelpMyStreet.Utils.Enums.SupportActivities>{HelpMyStreet.Utils.Enums.SupportActivities.CheckingIn},                    
                    ChampionPostcodes= new List<string>{ "NG1 1AE" }
                    
                }
            };

            _repository.Setup(x => x.GetVolunteersByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(_users);
        }

        [Test]
        public async Task WhenIGetStreetChampion_WithNoLinkedActivity_StreetChampionIsReturned()
        {

            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AE",
                    Activities = new List<HelpMyStreet.Utils.Enums.SupportActivities> { HelpMyStreet.Utils.Enums.SupportActivities.Shopping }
                }
            };

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Volunteers.Count());
            Assert.AreEqual(true, result.Volunteers.First().IsStreetChampionForGivenPostCode);

            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(It.Is<VolunteerType>(y => y == (VolunteerType.Helper | VolunteerType.StreetChampion)), It.Is<IsVerifiedType>(y => y == IsVerifiedType.All), It.IsAny<CancellationToken>()));
            _repository.Verify(x => x.GetVolunteersByIdsAsync(It.Is<IEnumerable<int>>(y => y.Count() == 1 && y.Any(z => z == 1))));
            _addressService.Verify(x => x.GetPostcodeCoordinatesAsync(It.Is<GetPostcodeCoordinatesRequest>(y => y.Postcodes.Count() == 1 && y.Postcodes.Contains("NG1 1AE")), It.IsAny<CancellationToken>()));
        }


        [Test]
        public async Task WhenIGetHelper_WithNoStreetChampionOrNoLinkedActivity_NoUserIsReturned()
        {

            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest()
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AA",
                    Activities = new List<HelpMyStreet.Utils.Enums.SupportActivities> { HelpMyStreet.Utils.Enums.SupportActivities.Shopping }
                }
            };

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);
            Assert.AreEqual(0, result.Volunteers.Count());                   
        }


        [Test]
        public async Task WhenIGetHelper_WithNoStreetChampionButWithLinkedActivity_IGetOneUserReturened()
        {
            _users = new List<User>()
            {
                new User()
                {
                    ID = 1,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test",
                        EmailAddress = "test@test.com"
                    },
                    SupportActivities = new List<HelpMyStreet.Utils.Enums.SupportActivities>{HelpMyStreet.Utils.Enums.SupportActivities.Shopping},
                    ChampionPostcodes= new List<string>{ "NG1 1AE" }

                }
            };
            _repository.Setup(x => x.GetVolunteersByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(_users);

            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest()
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AA",
                    Activities = new List<HelpMyStreet.Utils.Enums.SupportActivities> { HelpMyStreet.Utils.Enums.SupportActivities.Shopping }
                }                               
            };

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Volunteers.Count());            
        }
    }
}
