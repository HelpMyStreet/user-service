using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Models;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Config;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;

namespace UserService.UnitTests
{
    public class VolunteersForCacheGetterTests
    {
        private Mock<IRepository> _repository;
        private Mock<IAddressService> _addressService;
        private Mock<IOptionsSnapshot<ApplicationConfig>> _applicationConfig;


        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();

            IEnumerable<VolunteerForCacheDto> helperPostcodeRadiusDtos1 = new List<VolunteerForCacheDto>(){
            new VolunteerForCacheDto()
            {
                UserId = 1,
                Postcode = "NG1 1AA",
                SupportRadiusMiles = 1.2,
                VolunteerType = VolunteerType.StreetChampion,
                IsVerifiedType = IsVerifiedType.IsVerified,
                Latitude = 1,
                Longitude = 2,
                
            }

            };

            IEnumerable<VolunteerForCacheDto> helperPostcodeRadiusDtos2 = new List<VolunteerForCacheDto>(){
                new VolunteerForCacheDto()
                {
                    UserId =2,
                    Postcode = "NG1 1AB",
                    SupportRadiusMiles = 2.4,
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsVerified,
                    Latitude = 3,
                    Longitude = 4,
                }

            };

            _repository.SetupSequence(x => x.GetVolunteersForCacheAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(helperPostcodeRadiusDtos1)
                .ReturnsAsync(helperPostcodeRadiusDtos2);


            _repository.Setup(x => x.GetMinUserIdAsync()).ReturnsAsync(100);
            _repository.Setup(x => x.GetMaxUserIdAsync()).ReturnsAsync(250);
            _repository.Setup(x => x.GetDistinctVolunteerUserCount()).Returns(150);
            _repository.Setup(x => x.GetAllDistinctVolunteerUserCount()).Returns(150);
            IEnumerable<User> users = new List<User>(){
                new User()
            {
                ID = 1
            }
            };

            _repository.Setup(x => x.GetVolunteersByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(users);

            _addressService = new Mock<IAddressService>();


            ApplicationConfig applicationConfig = new ApplicationConfig()
            {
                GetVolunteersForCacheBatchSize = 100
            };

            _applicationConfig = new Mock<IOptionsSnapshot<ApplicationConfig>>();

            _applicationConfig.SetupGet(x => x.Value).Returns(applicationConfig);
        }

        [Test]
        public async Task GetHelpersByPostcode()
        {
            VolunteersForCacheGetter volunteersolunteersForCacheGetter = new VolunteersForCacheGetter(_repository.Object, _applicationConfig.Object);

            IEnumerable<CachedVolunteerDto> result = await volunteersolunteersForCacheGetter.GetAllVolunteersAsync(CancellationToken.None);

            // check batch sizes are calculated correctly
            _repository.Verify(x => x.GetVolunteersForCacheAsync(It.Is<int>(y => y == 100), It.Is<int>(y => y == 199)), Times.Once);
            _repository.Verify(x => x.GetVolunteersForCacheAsync(It.Is<int>(y => y == 200), It.Is<int>(y => y == 299)), Times.Once);


            _repository.Verify(x => x.GetVolunteersForCacheAsync(It.Is<int>(y => y == 100), It.Is<int>(y => y == 199)), Times.Once);
            _repository.Verify(x => x.GetVolunteersForCacheAsync(It.Is<int>(y => y == 200), It.Is<int>(y => y == 299)), Times.Once);


            Assert.AreEqual(2, result.Count());

            Assert.AreEqual("NG1 1AA", result.FirstOrDefault(x => x.UserId == 1).Postcode);
            Assert.AreEqual(1.2, result.FirstOrDefault(x => x.UserId == 1).SupportRadiusMiles);
            Assert.AreEqual(1, result.FirstOrDefault(x => x.UserId == 1).Latitude);
            Assert.AreEqual(2, result.FirstOrDefault(x => x.UserId == 1).Longitude);
            Assert.AreEqual(VolunteerType.StreetChampion, result.FirstOrDefault(x => x.UserId == 1).VolunteerType);
            Assert.AreEqual(IsVerifiedType.IsVerified, result.FirstOrDefault(x => x.UserId == 1).IsVerifiedType);

            Assert.AreEqual("NG1 1AB", result.FirstOrDefault(x => x.UserId == 2).Postcode);
            Assert.AreEqual(2.4, result.FirstOrDefault(x => x.UserId == 2).SupportRadiusMiles);
            Assert.AreEqual(3, result.FirstOrDefault(x => x.UserId == 2).Latitude);
            Assert.AreEqual(4, result.FirstOrDefault(x => x.UserId == 2).Longitude);
            Assert.AreEqual(VolunteerType.Helper, result.FirstOrDefault(x => x.UserId == 2).VolunteerType);
            Assert.AreEqual(IsVerifiedType.IsVerified, result.FirstOrDefault(x => x.UserId == 2).IsVerifiedType);

        }

        [Test]
        public async Task NoVolunteersInDbReturnsAnEmptyList()
        {
            _repository.Setup(x => x.GetAllDistinctVolunteerUserCount()).Returns(0);

            VolunteersForCacheGetter volunteersolunteersForCacheGetter = new VolunteersForCacheGetter(_repository.Object, _applicationConfig.Object);

            IEnumerable<CachedVolunteerDto> result = await volunteersolunteersForCacheGetter.GetAllVolunteersAsync(CancellationToken.None);

            Assert.AreEqual(0, result.Count());

        }
    }
}