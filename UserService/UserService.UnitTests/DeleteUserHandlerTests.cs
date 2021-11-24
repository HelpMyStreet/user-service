using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.GroupService.Request;
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
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class DeleteUserHandlerTests
    {
        private DeleteUserHandler _classUnderTest;
        private Mock<IRepository> _repository;
        private Mock<IAuthService> _authService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<IGroupService> _groupService;
        private Dictionary<int, List<int>> _userRoles;

        private User _user;
        private bool _authSuccess;
        private bool _success;
        private bool _deleteContact;

        [SetUp]
        public void SetUp()
        {
            SetupRepository();
            SetupAuthService();
            SetupCommunicationService();
            SetupGroupService();
            _classUnderTest = new DeleteUserHandler(_repository.Object, _authService.Object, _communicationService.Object, _groupService.Object) ;
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetUserByID(It.IsAny<int>()))
                .Returns(() => _user);

            _repository.Setup(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _success);
        }

        private void SetupAuthService()
        {
            _authService = new Mock<IAuthService>();

            _authService.Setup(x => x.DeleteUser(It.IsAny<string>()))
                .ReturnsAsync(() => _authSuccess);
        }

        private void SetupCommunicationService()
        {
            _communicationService = new Mock<ICommunicationService>();
            _communicationService.Setup(x => x.DeleteMarketingContactAsync(It.IsAny<DeleteMarketingContactRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _deleteContact);
        }

        private void SetupGroupService()
        {
            _groupService = new Mock<IGroupService>();
            _groupService.Setup(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _userRoles);
        }

        [Test]
        public void Success()
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

            var result = _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.DeleteUserRequest()
            {
                Postcode = "POSTCODE",
                UserID = userId,
            }, CancellationToken.None).Result;

            Assert.AreEqual(result.Success, _success);
            _authService.Verify(x => x.DeleteUser(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.DeleteUserAsync(It.IsAny<int>(),It.IsAny<CancellationToken>()), Times.Once);
            _groupService.Verify(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _groupService.Verify(x => x.PostRevokeRole(It.IsAny<PostRevokeRoleRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(_userRoles.Sum(x => x.Value.Count)));
        }

        [Test]
        public void WhenUserNotFound_ReturnsFalse()
        {
            _user = null;
            _authSuccess = false;
            _success = false;

            var result = _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.DeleteUserRequest()
            {
                Postcode = "POSTCODE",
                UserID = 1
            }, CancellationToken.None).Result;

            Assert.AreEqual(result.Success, _success);
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

            var result = _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.DeleteUserRequest()
            {
                Postcode = "POSTCODE",
                UserID = 1
            }, CancellationToken.None).Result;

            Assert.AreEqual(result.Success, _success);
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

            var result = _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.DeleteUserRequest()
            {
                Postcode = "POSTCODE",
                UserID = 1
            }, CancellationToken.None).Result;

            Assert.AreEqual(result.Success, _success);
            _authService.Verify(x => x.DeleteUser(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.DeleteUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.GetUserRoles(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _groupService.Verify(x => x.PostRevokeRole(It.IsAny<PostRevokeRoleRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
