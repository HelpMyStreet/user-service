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
        [SetUp]
        public void SetUp()
        {
      
            _repository = new Mock<IRepository>();


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
            _helperService.Setup(x => x.GetHelpersWithinRadius(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _helpers);
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

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService.Object, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);
            Assert.AreEqual(0, result.Volunteers.Count());
            _helperService.Verify(X => X.GetHelpersWithinRadius("NG1 1AA", It.IsAny<CancellationToken>()));
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

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService.Object, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Volunteers.Count());
            _helperService.Verify(X => X.GetHelpersWithinRadius("NG1 1AA", It.IsAny<CancellationToken>()));
        }
    }
}
