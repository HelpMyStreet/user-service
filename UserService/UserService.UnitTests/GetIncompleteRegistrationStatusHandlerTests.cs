using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class GetIncompleteRegistrationStatusHandlerTests
    {
        private GetIncompleteRegistrationStatusHandler _classUnderTest;
        private List<UserRegistrationStep> _users;
        private Mock<IRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            SetupRepository();
            _classUnderTest = new GetIncompleteRegistrationStatusHandler(_repository.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();

            _repository.Setup(x => x.GetIncompleteRegistrationStatusAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _users);
        }

        [Test]
        public async Task GivenVolRegIncompleteThenVolsReturned()
        {
            _users = new List<UserRegistrationStep>()
            {
                new UserRegistrationStep()
                {
                    UserId = 1,
                    RegistrationStep = 2,
                    DateCompleted = DateTime.Now
                },
                new UserRegistrationStep()
                {
                    UserId = 2,
                    RegistrationStep = 2,
                    DateCompleted = DateTime.Now
                }
            };
            var request = new GetIncompleteRegistrationStatusRequest();
            var result = await _classUnderTest.Handle(request, CancellationToken.None);
            Assert.AreEqual(_users, result.Users);
        }

        
    }
}
