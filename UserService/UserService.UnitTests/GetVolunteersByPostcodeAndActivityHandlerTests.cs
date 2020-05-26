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
        private Mock<IHelperService> _helperService;
        private Mock<IRepository> _repository;

        private IEnumerable<HelperWithinRadiusDTO> _helpers;
        [SetUp]
        public void SetUp()
        {
      
            _repository = new Mock<IRepository>();


            _helpers = new List<HelperWithinRadiusDTO>
            { 
                new HelperWithinRadiusDTO { User =    new User
                {
                    ID = 1,
                    UserPersonalDetails = new UserPersonalDetails
                    {
                        DisplayName = "Test",
                        EmailAddress = "test@test.com"
                    },
                    SupportActivities = new List<HelpMyStreet.Utils.Enums.SupportActivities>{HelpMyStreet.Utils.Enums.SupportActivities.CheckingIn},
                    ChampionPostcodes= new List<string>{ "NG1 1AE" }

                },
                    Distance = 1,
                }
            };
           
            _helperService.Setup(x => x.GetHelpersWithinRadius(It.IsAny<string>(), It.IsAny<IsVerifiedType>(), It.IsAny<CancellationToken>())).ReturnsAsync(;
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

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService.Object, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Volunteers.Count());
            Assert.AreEqual(true, result.Volunteers.First().IsStreetChampionForGivenPostCode);

           
            _repository.Verify(x => x.GetVolunteersByIdsAsync(It.Is<IEnumerable<int>>(y => y.Count() == 1 && y.Any(z => z == 1))));
           
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

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService.Object, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);
            Assert.AreEqual(0, result.Volunteers.Count());                   
        }


        [Test]
        public async Task WhenIGetHelper_WithNoStreetChampionButWithLinkedActivity_IGetOneUserReturened()
        {
            

            GetVolunteersByPostcodeAndActivityRequest request = new GetVolunteersByPostcodeAndActivityRequest()
            {
                VolunteerFilter = new VolunteerFilter
                {
                    Postcode = "NG1 1AA",
                    Activities = new List<HelpMyStreet.Utils.Enums.SupportActivities> { HelpMyStreet.Utils.Enums.SupportActivities.Shopping }
                }                               
            };

            GetVolunteersByPostcodeAndActivityHandler getVolunteersByPostcodeAndActivityHandler = new GetVolunteersByPostcodeAndActivityHandler(_helperService.Object, _repository.Object);

            GetVolunteersByPostcodeAndActivityResponse result = await getVolunteersByPostcodeAndActivityHandler.Handle(request, CancellationToken.None);

            Assert.AreEqual(1, result.Volunteers.Count());            
        }
    }
}
