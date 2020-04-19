using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using UserService.Core.Config;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetHelpersByPostcodeHandlerTests
    {

        private Mock<IRepository> _repository;
        private Mock<IAddressService> _addressService;
        private Mock<IOptionsSnapshot<ApplicationConfig>> _applicationConfig;

        private readonly string _postcode = "NG1 5FS";

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();

            IEnumerable<HelperPostcodeRadiusDto> helperPostcodeRadiusDtos1 = new List<HelperPostcodeRadiusDto>(){
            new HelperPostcodeRadiusDto()
            {
                UserId = 1,
                Postcode = "NG1 5FS",
                SupportRadiusMiles = DistanceConverter.MilesToMetres(1)
            }

            };

            IEnumerable<HelperPostcodeRadiusDto> helperPostcodeRadiusDtos2 = new List<HelperPostcodeRadiusDto>(){
                new HelperPostcodeRadiusDto()
                {
                    UserId =2,
                    Postcode = "NG1 6Dq",
                    SupportRadiusMiles = DistanceConverter.MilesToMetres(2)
                }

            };

            _repository.SetupSequence(x => x.GetVolunteersPostcodeRadiiAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(helperPostcodeRadiusDtos1)
                .ReturnsAsync(helperPostcodeRadiusDtos2);


            _repository.Setup(x => x.GetMinUserIdAsync()).ReturnsAsync(100);
            _repository.Setup(x => x.GetMaxUserIdAsync()).ReturnsAsync(250);

            IEnumerable<User> users = new List<User>(){
                new User()
            {
                ID = 1
            }

            };

            _repository.Setup(x => x.GetVolunteersByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(users);

            _addressService = new Mock<IAddressService>();

            IsPostcodeWithinRadiiResponse isPostcodeWithinRadiiBatch1 = new IsPostcodeWithinRadiiResponse()
            {
                IdsWithinRadius = new List<int>() { 1 }
            };

            IsPostcodeWithinRadiiResponse isPostcodeWithinRadiiBatch2 = new IsPostcodeWithinRadiiResponse()
            {
                IdsWithinRadius = new List<int>() { 2 }
            };

            _addressService.SetupSequence(x => x.IsPostcodeWithinRadiiAsync(It.IsAny<IsPostcodeWithinRadiiRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(isPostcodeWithinRadiiBatch1)
                .ReturnsAsync(isPostcodeWithinRadiiBatch2);

            var applicationConfig = new ApplicationConfig()
            {
                GetHelpersByPostcodeBatchSize = 100
            };

            _applicationConfig = new Mock<IOptionsSnapshot<ApplicationConfig>>();

            _applicationConfig.SetupGet(x => x.Value).Returns(applicationConfig);
        }

        [Test]
        public async Task GetHelpersByPostcode()
        {
            GetHelpersByPostcodeRequest request = new GetHelpersByPostcodeRequest()
            {
                PostCode = _postcode
            };

            GetHelpersByPostcodeHandler getHelpersByPostcodeHandler = new GetHelpersByPostcodeHandler(_repository.Object, _addressService.Object, _applicationConfig.Object);

            GetHelpersByPostcodeResponse result = await getHelpersByPostcodeHandler.Handle(request, CancellationToken.None);

            // check batch sizes are calculated correctly
            _repository.Verify(x => x.GetVolunteersPostcodeRadiiAsync(It.Is<int>(y => y == 100), It.Is<int>(y => y == 199)), Times.Once);
            _repository.Verify(x => x.GetVolunteersPostcodeRadiiAsync(It.Is<int>(y => y == 200), It.Is<int>(y => y == 299)), Times.Once);


            _addressService.Verify(x => x.IsPostcodeWithinRadiiAsync(It.Is<IsPostcodeWithinRadiiRequest>(
                y => y.Postcode == _postcode &&
                    y.PostcodeWithRadiuses.Count() == 1 &&
                    y.PostcodeWithRadiuses.Any(z => z.Id == 1)
                ), It.IsAny<CancellationToken>()), Times.Once);

            _addressService.Verify(x => x.IsPostcodeWithinRadiiAsync(It.Is<IsPostcodeWithinRadiiRequest>(
                y => y.Postcode == _postcode &&
                     y.PostcodeWithRadiuses.Count() == 1 &&
                     y.PostcodeWithRadiuses.Any(z => z.Id == 2)
            ), It.IsAny<CancellationToken>()), Times.Once);


            _repository.Verify(x => x.GetVolunteersByIdsAsync(It.Is<IEnumerable<int>>(y=> 
                y.Count() == 2 &&
                y.Contains(1)  &&
                y.Contains(2)
                )));

            Assert.AreEqual(1, result.Users.Count);
            Assert.AreEqual(1, result.Users.FirstOrDefault().ID);
        }
    }
}