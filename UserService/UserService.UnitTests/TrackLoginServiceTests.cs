using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Utils.Models;
using Microsoft.Extensions.Internal;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UserService.Core.Contracts;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Services;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class TrackLoginServiceTests
    {
        private TrackLoginService _classUnderTest;

        private Mock<ISystemClock> _systemClockMock;
        private Mock<IAuthService> _authService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<IRepository> _repository;
        private Mock<IGroupService> _groupService;

        private User _user;
        private List<UserLoginHistory> _users;
        private Dictionary<int, List<int>> _userRoles;
        private bool _success;
        private bool _authSuccess;
        private bool _deleteContact;
        private DateTime? _dateLastEmailSent;

        [SetUp]
        public void SetUp()
        {
            _systemClockMock = new Mock<ISystemClock>();
            _systemClockMock.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 8, 1, 10, 0, 0, TimeSpan.Zero));
            SetupRepository();
            SetupAuthService();
            SetupCommunicationService();
            SetupGroupService();
            _classUnderTest = new TrackLoginService(_systemClockMock.Object, _authService.Object, _communicationService.Object, _repository.Object, _groupService.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();

            _repository.Setup(x => x.GetInactiveUsers(It.IsAny<int>()))
                .ReturnsAsync(() => _users);

            _repository.Setup(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _success);

            _repository.Setup(x => x.GetUserByID(It.IsAny<int>()))
                .Returns(() => _user);
        }

        private void SetupAuthService()
        {
            _authService = new Mock<IAuthService>();

            _authService.Setup(x => x.DeleteUser(It.IsAny<string>()))
                .ReturnsAsync(() => _authSuccess);
        }

        private void SetupGroupService()
        {
            _groupService = new Mock<IGroupService>();

            _groupService.Setup(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _userRoles);
        }

        private void SetupCommunicationService()
        {
            _communicationService = new Mock<ICommunicationService>();
            _communicationService.Setup(x => x.RequestCommunicationAsync(It.IsAny<RequestCommunicationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _success);

            _communicationService.Setup(x => x.DeleteMarketingContactAsync(It.IsAny<DeleteMarketingContactRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _deleteContact);

            _communicationService.Setup(x => x.GetDateEmailLastSentAsync(It.IsAny<GetDateEmailLastSentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _dateLastEmailSent);
        }


        [TestCase(1, "2002-01-10", null)]
        [TestCase(2, "2020-01-10", "2022-01-11")]
        [Test]
        public void WhenUserHasBeenInactive_AndHasNotBeenNotified_SendEmail(int userId, DateTime dtLastLogin, DateTime? dtLastEmailSent)
        {
            _users = new List<UserLoginHistory>();
            _users.Add(new UserLoginHistory() { UserId = userId, Postcode = "POSTCODE", DateLastLogin = dtLastLogin});
            _success = true;
            _dateLastEmailSent = dtLastEmailSent;

            var result = _classUnderTest.ManageInactiveUsers(2);

            if (!dtLastEmailSent.HasValue)
            {
                _communicationService.Verify(
                    x => x.RequestCommunicationAsync(
                        It.Is<RequestCommunicationRequest>(
                            arg => arg.RecipientUserID == userId &&
                            arg.AdditionalParameters.ContainsKey("LastActiveDate") &
                            arg.CommunicationJob.CommunicationJobType == HelpMyStreet.Contracts.RequestService.Response.CommunicationJobTypes.ImpendingUserDeletion
                    )
                 , It.IsAny<CancellationToken>()), Times.Once);

                _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Never);
            }
        }

        [Test]
        public void WhenEmptyListOfInactiveUsers_SendNoEmails()
        {
            _users = new List<UserLoginHistory>();

            var result = _classUnderTest.ManageInactiveUsers(2);

            _communicationService.Verify(
                x => x.RequestCommunicationAsync(It.IsAny<RequestCommunicationRequest>(),It.IsAny<CancellationToken>())
                ,Times.Never);
        }

        [Test]
        public void WhenMultipleInactiveUsersWhoHaveNotBeenSentEmail_SendCorrectNumberOfEmails()
        {
            _dateLastEmailSent = null;
            _users = new List<UserLoginHistory>();
            _users.Add(new UserLoginHistory() { UserId = 1, Postcode = "POSTCODE", DateLastLogin = new DateTime(2020, 1, 1) });
            _users.Add(new UserLoginHistory() { UserId = 2, Postcode = "POSTCODE", DateLastLogin = new DateTime(2020, 2, 1) });

            var result = _classUnderTest.ManageInactiveUsers(2);

            _communicationService.Verify(
                x => x.RequestCommunicationAsync(
                    It.Is<RequestCommunicationRequest>(
                        arg => _users.Select(x=>x.UserId).ToList().Contains(arg.RecipientUserID.Value) &&
                        arg.AdditionalParameters.ContainsKey("LastActiveDate") &
                        arg.CommunicationJob.CommunicationJobType == HelpMyStreet.Contracts.RequestService.Response.CommunicationJobTypes.ImpendingUserDeletion
                )
             , It.IsAny<CancellationToken>()), Times.Exactly(_users.Count));

            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void WhenInactiveHasBeenSentEmail_AndIsTimeToDelete_Then_DeleteAndSendEmail()
        {
            int userId = 1;
            _dateLastEmailSent = new DateTime(2020,8,1).AddDays(-30);
            _users = new List<UserLoginHistory>();
            _users.Add(new UserLoginHistory() { UserId = userId, Postcode = "POSTCODE", DateLastLogin = new DateTime(2020, 1, 1) });

            var result = _classUnderTest.ManageInactiveUsers(2);

            _communicationService.Verify(
                x => x.RequestCommunicationAsync(
                    It.Is<RequestCommunicationRequest>(
                        arg => _users.Select(x => x.UserId).ToList().Contains(arg.RecipientUserID.Value) &&
                        arg.AdditionalParameters.ContainsKey("LastActiveDate") &
                        arg.CommunicationJob.CommunicationJobType == HelpMyStreet.Contracts.RequestService.Response.CommunicationJobTypes.ImpendingUserDeletion
                )
             , It.IsAny<CancellationToken>()), Times.Exactly(0));

            _repository.Verify(x => x.GetUserByID(userId), Times.Once);
        }

        [Test]
        public void SuccessfullyDeleteUser()
        {
            int userId = 1;
            _user = new User()
            {
                PostalCode = "POSTCODE",
                UserPersonalDetails = new UserPersonalDetails()
                {
                    EmailAddress = "EMAILADDRESS"
                }
            };
            _authSuccess = true;
            _success = true;
            _deleteContact = true;
            _userRoles = new Dictionary<int, List<int>>()
            {
                {-1,new List<int>(){1,2,3 } },
                {-2,new List<int>(){1,2,3 } }
            };

            var result = _classUnderTest.DeleteUser(userId, "POSTCODE", true, CancellationToken.None).Result;

            Assert.AreEqual(result, _success);
            _authService.Verify(x => x.DeleteUser(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _groupService.Verify(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _groupService.Verify(x => x.PostRevokeRole(It.IsAny<PostRevokeRoleRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(_userRoles.Sum(x => x.Value.Count)));
        }

        [Test]
        public void WhenUserNotFound_ReturnsFalse()
        {
            _user = null;
            _authSuccess = false;
            _success = false;

            var result = _classUnderTest.DeleteUser(1, "POSTCODE", true, CancellationToken.None).Result;

            Assert.AreEqual(result, _success);
            _authService.Verify(x => x.DeleteUser(It.IsAny<string>()), Times.Never);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.PostRevokeRole(It.IsAny<PostRevokeRoleRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void WhenUserFoundPostCodeDoesNotMatch_ReturnsFalse()
        {
            _user = new User()
            {
                PostalCode = "NG1 6DQ"
            };
            _authSuccess = false;
            _success = false;

            var result = _classUnderTest.DeleteUser(1, "POSTCODE", true, CancellationToken.None).Result;

            Assert.AreEqual(result, _success);
            _authService.Verify(x => x.DeleteUser(It.IsAny<string>()), Times.Never);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.PostRevokeRole(It.IsAny<PostRevokeRoleRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void WhenFirebaseUserCannotBeDeleted_ReturnFalse()
        {
            _user = new User()
            {
                PostalCode = "POSTCODE"
            };
            _authSuccess = false;
            _success = false;

            var result = _classUnderTest.DeleteUser(1, "POSTCODE", true, CancellationToken.None).Result;

            Assert.AreEqual(result, _success);
            _authService.Verify(x => x.DeleteUser(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.PostRevokeRole(It.IsAny<PostRevokeRoleRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
