using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetVolunteersByPostcodeAndActivityHandlerTests
    {                        
        private Mock<IHelperService> _helperService;
        private Mock<IRepository> _repository;
        private IEnumerable<HelperWithinRadiusDTO> _helpers;

        private GetVolunteersByPostcodeAndActivityHandler _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            SetUpRepository();
            SetUpHelperService();

            _classUnderTest = new GetVolunteersByPostcodeAndActivityHandler(_helperService.Object, _repository.Object);
        }

        private void SetUpRepository()
        {
            _repository = new Mock<IRepository>();
        }

        private void SetUpHelperService()
        {
            _helpers = new List<HelperWithinRadiusDTO>
            {
                new HelperWithinRadiusDTO {
                    User =    new User
                {
                    ID = 1,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test",
                        EmailAddress = "test@test.com"
                    },
                    SupportActivities = new List<SupportActivities>{ SupportActivities.Shopping},

                },
                    Distance = 1,
                }
            };
            _helperService = new Mock<IHelperService>();
            _helperService.Setup(x => x.GetHelpersWithinRadius(It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _helpers);
        }

        [Test]
        public async Task WhenIGetHelper_WithNoStreetChampionOrNoLinkedActivity_NoUserIsReturned()
        {
            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest()
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AA",
                    Activities = new List<SupportActivities> { SupportActivities.CheckingIn }
                }
            };

            var result = await _classUnderTest.Handle(request, CancellationToken.None);

            Assert.AreEqual(0, result.Volunteers.Count());
            _helperService.Verify(X => X.GetHelpersWithinRadius("NG1 1AA", It.IsAny<double?>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task WhenIGetHelper_WithNoStreetChampionButWithLinkedActivity_IGetOneUserReturened()
        {
            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest()
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AA",
                    Activities = new List<SupportActivities> { SupportActivities.Shopping }
                }                               
            };

            var result = await _classUnderTest.Handle(request, CancellationToken.None);
            Assert.AreEqual(1, result.Volunteers.Count());
            _helperService.Verify(X => X.GetHelpersWithinRadius("NG1 1AA", It.IsAny<double?>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GivenMultipleVolsOnlyVolsWithCorrectActivityReturned()
        {
            _helpers = new List<HelperWithinRadiusDTO>
            {
                new HelperWithinRadiusDTO {
                    User =    new User
                {
                    ID = 1,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test",
                        EmailAddress = "test@test.com"
                    },
                    SupportActivities = new List<SupportActivities>{ SupportActivities.Shopping},

                },
                    Distance = 1,
                },
                new HelperWithinRadiusDTO {
                    User =    new User
                {
                    ID = 2,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test2",
                        EmailAddress = "test2@test.com"
                    },
                    SupportActivities = new List<SupportActivities>{ SupportActivities.CheckingIn},

                },
                    Distance = 1,
                }
            };

            SupportActivities sa = SupportActivities.Shopping;

            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest()
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AA",
                    Activities = new List<SupportActivities> { sa }
                }
            };

            var result = await _classUnderTest.Handle(request, CancellationToken.None);
            Assert.AreEqual(
                _helpers.Where(x=> x.User.SupportActivities.Contains(sa)).Select(x=> x.User.ID).ToList(), 
                result.Volunteers.Select(x=> x.UserID).ToList());
        }

    }
}
