using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Services;
using UserService.Core.Utils;

namespace UserService.UnitTests
{
    public class HelperServiceTests {

        private Mock<IVolunteerCache> _volunteerCache;
        private Mock<IDistanceCalculator> _distanceCalculator;        
        private IHelperService _helperService;
        private Mock<IRepository> _repository;
        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtos;
        private IEnumerable<User> _users;
        private int _distanceInMiles;
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
                    IsVerified = true,
                    SupportActivities = new List<SupportActivities>{ SupportActivities.Shopping},
                    ChampionPostcodes= new List<string>{ "NG1 1AE" }
                },
                             new User()
                {
                    ID = 2,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test",
                        EmailAddress = "test@test.com"
                    },
                    IsVerified = false,
                    SupportActivities = new List<SupportActivities>{ SupportActivities.Shopping},
                    ChampionPostcodes= new List<string>{ "NG1 1AB" }

                }
            };

            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetVolunteersByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(() => _users);
            _repository.Setup(x => x.GetLatitudeAndLongitude(It.IsAny<string>())).Returns(new LatitudeAndLongitudeDTO
            {
                Latitude = 1,
                Longitude = 2,
            });

            _volunteerCache = new Mock<IVolunteerCache>();
            _volunteerCache.Setup(x => x.GetCachedVolunteersAsync(It.IsAny<VolunteerType>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);

            _distanceCalculator = new Mock<IDistanceCalculator>();
            _distanceInMiles = 1;
            _distanceCalculator.Setup(x => x.GetDistanceInMiles(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(() => _distanceInMiles);
            _helperService = new HelperService(_volunteerCache.Object, _distanceCalculator.Object, _repository.Object);

        }

        [Test]
        public async Task WhenICall_WithHappyPath_ICallRequiredFunctions()
        {
            var users = await _helperService.GetHelpersWithinRadius("T35T 3TY", IsVerifiedType.All, new CancellationToken());
            _repository.Verify(x => x.GetLatitudeAndLongitude("T35T 3TY"), Times.Once);
            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(VolunteerType.Helper | VolunteerType.StreetChampion, IsVerifiedType.All, It.IsAny<CancellationToken>()), Times.Once);
            _distanceCalculator.Verify(x => x.GetDistanceInMiles(1, 2, 1, 2), Times.Once);
            _distanceCalculator.Verify(x => x.GetDistanceInMiles(1, 2, 3, 4), Times.Once);
            _repository.Verify(x => x.GetVolunteersByIdsAsync(new List<int> { 1, 2 }), Times.Once);
        }

        [Test]
        public async Task WhenICall_WithHelpers_OutsideOfSupportRadius_IGetNoUsers()
        {
            _distanceInMiles = 5;
            _users = new List<User>();

            var users = await _helperService.GetHelpersWithinRadius("T35T 3TY", IsVerifiedType.All, new CancellationToken());
            Assert.AreEqual(0, users.Count());
        }

        [Test]
        public async Task WhenICall_WithHelpers_InsideSupportRadius_IGetUsers()
        {
 
            var users = await _helperService.GetHelpersWithinRadius("T35T 3TY", IsVerifiedType.All, new CancellationToken());
            Assert.AreEqual(2, users.Count());
        }

        [Test]
        public async Task WhenICall_WithIsVerifiedType_EqualsFalse_IGetUnverifiedUsers()
        {
            var users =  await _helperService.GetHelpersWithinRadius("T35T 3TY", IsVerifiedType.IsNotVerified, new CancellationToken());
            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(VolunteerType.Helper | VolunteerType.StreetChampion, IsVerifiedType.IsNotVerified, It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual(false, users.First().User.IsVerified);

        }
        [Test]
        public async Task WhenICall_WithIsVerifiedType_EqualsTrue_IGetVerifiedUsers()
        {
            var users = await _helperService.GetHelpersWithinRadius("T35T 3TY", IsVerifiedType.IsVerified, new CancellationToken());
            _volunteerCache.Verify(x => x.GetCachedVolunteersAsync(VolunteerType.Helper | VolunteerType.StreetChampion, IsVerifiedType.IsVerified, It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual(true, users.First().User.IsVerified);
        }
    }
}
