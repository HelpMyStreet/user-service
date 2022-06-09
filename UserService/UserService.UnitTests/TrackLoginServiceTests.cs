using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Services;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class TrackLoginServiceTests
    {
        private TrackLoginService _classUnderTest;

        private Mock<IAuthService> _authService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<IRepository> _repository;

        private List<Tuple<int, DateTime?>> _users;
        private bool _success;

        [SetUp]
        public void SetUp()
        {
            SetupRepository();
            SetupAuthService();
            SetupCommunicationService();
            _classUnderTest = new TrackLoginService(_authService.Object, _communicationService.Object, _repository.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetInactiveUsers(It.IsAny<int>()))
                .ReturnsAsync(() => _users);
        }

        private void SetupAuthService()
        {
            _authService = new Mock<IAuthService>();
        }

        private void SetupCommunicationService()
        {
            _communicationService = new Mock<ICommunicationService>();
            _communicationService.Setup(x => x.RequestCommunicationAsync(It.IsAny<RequestCommunicationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _success);
        }


        [TestCase(1, "2002-01-01")]
        [Test]
        public void Success(int userId, DateTime dtLastLogin)
        {
            _users = new List<Tuple<int, DateTime?>>();
            _users.Add(new Tuple<int, DateTime?>(userId, dtLastLogin));
            _success = true;

            var result = _classUnderTest.NotifyInactiveUsers(2);
            
            _communicationService.Verify(
                x => x.RequestCommunicationAsync(
                    It.Is<RequestCommunicationRequest>(
                        arg => arg.RecipientUserID == userId &&
                        arg.AdditionalParameters.ContainsKey("LastActiveDate") &
                        arg.CommunicationJob.CommunicationJobType == HelpMyStreet.Contracts.RequestService.Response.CommunicationJobTypes.ImpendingUserDeletion
                )
             ,It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
