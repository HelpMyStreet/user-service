using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class PutModifyRegistrationPageThreeHandlerTests
    {
        private PutModifyRegistrationPageThreeHandler _classUnderTest;
        private Mock<IRepository> _repository;
        private Mock<ICommunicationService> _communicationService;
        private Mock<IGroupService> _groupService;
        private User _user;
        private int _userId;
        private List<SupportActivityConfiguration> _supportActivityConfigurations;
        private bool _emailSent;

        [SetUp]
        public void SetUp()
        {
            SetupRepository();
            SetupCommunicationService();
            SetupGroupService();
            _classUnderTest = new PutModifyRegistrationPageThreeHandler(_repository.Object, _communicationService.Object, _groupService.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();

            _repository.Setup(x => x.ModifyUserRegistrationPageThree(It.IsAny<RegistrationStepThree>()))
                .Returns(() => _userId);
        }

        private void SetupCommunicationService()
        {
            _communicationService = new Mock<ICommunicationService>();
            _communicationService.Setup(x => x.RequestCommunicationAsync(It.IsAny<RequestCommunicationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _emailSent);
        }

        private void SetupGroupService()
        {
            _groupService = new Mock<IGroupService>();

            _supportActivityConfigurations = new List<SupportActivityConfiguration>()
            {
               new SupportActivityConfiguration()
               {
                   SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Shopping,
                   AutoSignUpWhenOtherSelected = true
               },
               new SupportActivityConfiguration()
               {
                   SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Errands,
                   AutoSignUpWhenOtherSelected = true
               }
            };

            _groupService.Setup(x => x.GetSupportActivitiesConfigurationAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _supportActivityConfigurations);
        }

        [Test]
        public void WhenOtherActivitySelected_AddAdditionalActivities()
        {
            _emailSent = true;
            _userId = 1;

            var result = _classUnderTest.Handle(new PutModifyRegistrationPageThreeRequest()
            {
                RegistrationStepThree = new RegistrationStepThree()
                {
                    Activities = new List<HelpMyStreet.Utils.Enums.SupportActivities>
                    {
                        HelpMyStreet.Utils.Enums.SupportActivities.DogWalking
                    }
                }
            },CancellationToken.None);
            _groupService.Verify(x => x.GetSupportActivitiesConfigurationAsync(It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(x => x.ModifyUserRegistrationPageThree(It.IsAny<RegistrationStepThree>()), Times.Once);
            _communicationService.Verify(x => x.RequestCommunicationAsync(It.IsAny<RequestCommunicationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void WhenNonOtherActivitySelected_AddAdditionalActivities()
        {
            _emailSent = true;
            _userId = 1;

            var result = _classUnderTest.Handle(new PutModifyRegistrationPageThreeRequest()
            {
                RegistrationStepThree = new RegistrationStepThree()
                {
                    Activities = new List<HelpMyStreet.Utils.Enums.SupportActivities>
                    {
                        HelpMyStreet.Utils.Enums.SupportActivities.Other
                    }
                }
            }, CancellationToken.None);
            _groupService.Verify(x => x.GetSupportActivitiesConfigurationAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(x => x.ModifyUserRegistrationPageThree(It.IsAny<RegistrationStepThree>()), Times.Once);
            _communicationService.Verify(x => x.RequestCommunicationAsync(It.IsAny<RequestCommunicationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
