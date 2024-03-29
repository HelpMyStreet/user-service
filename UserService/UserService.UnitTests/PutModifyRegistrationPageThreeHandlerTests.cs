﻿using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Utils.Enums;
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
        private Mock<IGroupService> _groupService;     
        private int _userId;
        private List<SupportActivityConfiguration> _supportActivityConfigurations;
        private bool _emailSent;
        private List<SupportActivityDetail> _supportActivityDetails;


        [SetUp]
        public void SetUp()
        {
            SetupRepository();
            SetupGroupService();
            _classUnderTest = new PutModifyRegistrationPageThreeHandler(_repository.Object, _groupService.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();

            _repository.Setup(x => x.ModifyUserRegistrationPageThree(It.IsAny<RegistrationStepThree>()))
                .Returns(() => _userId);
        }

        private void SetupGroupService()
        {
            _groupService = new Mock<IGroupService>();

            _supportActivityConfigurations = new List<SupportActivityConfiguration>()
            {
               new SupportActivityConfiguration()
               {
                   SupportActivity = SupportActivities.Shopping,
                   AutoSignUpWhenOtherSelected = true
               },
               new SupportActivityConfiguration()
               {
                   SupportActivity = SupportActivities.Errands,
                   AutoSignUpWhenOtherSelected = true
               }
            };

            _groupService.Setup(x => x.GetSupportActivitiesConfigurationAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _supportActivityConfigurations);

            _supportActivityDetails = new List<SupportActivityDetail>()
            {
                new SupportActivityDetail()
                {
                    SupportActivity = SupportActivities.CommunityConnector
                },
                new SupportActivityDetail()
                {
                    SupportActivity = SupportActivities.CollectingPrescriptions
                },
                new SupportActivityDetail()
                {
                   SupportActivity = SupportActivities.Errands,
                }
            };

            _groupService.Setup(x => x.GetRegistrationFormSupportActivities(It.IsAny<RegistrationFormVariant>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _supportActivityDetails);
        }

        [Test]
        public void WhenNonOtherActivitySelected_DoNotAdditionalActivities()
        {
            _emailSent = true;
            _userId = 1;

            var result = _classUnderTest.Handle(new PutModifyRegistrationPageThreeRequest()
            {
                RegistrationStepThree = new RegistrationStepThree()
                {
                    Activities = new List<SupportActivities>
                    {
                        SupportActivities.DogWalking
                    },
                    RegistrationFormVariant = RegistrationFormVariant.Default
                }
            },CancellationToken.None);
            _groupService.Verify(x => x.GetSupportActivitiesConfigurationAsync(It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.GetRegistrationFormSupportActivities(It.IsAny<RegistrationFormVariant>(), It.IsAny<CancellationToken>()), Times.Never);
            _repository.Verify(x => x.ModifyUserRegistrationPageThree(It.IsAny<RegistrationStepThree>()), Times.Once);
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
                    Activities = new List<SupportActivities>
                    {
                        SupportActivities.Other
                    },
                    RegistrationFormVariant = RegistrationFormVariant.Default
                }
            }, CancellationToken.None);
            _groupService.Verify(x => x.GetSupportActivitiesConfigurationAsync(It.IsAny<CancellationToken>()), Times.Once);
            _groupService.Verify(x => x.GetRegistrationFormSupportActivities(It.IsAny<RegistrationFormVariant>(), It.IsAny<CancellationToken>()), Times.Once);
            _repository.Verify(x => x.ModifyUserRegistrationPageThree(It.IsAny<RegistrationStepThree>()), Times.Once);
        }

    }
}
