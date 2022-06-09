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
        private Mock<ITrackLoginService> _trackLoginService;
        private bool _success;

        [SetUp]
        public void SetUp()
        {
            _trackLoginService = new Mock<ITrackLoginService>();

            _trackLoginService.Setup(x => x.DeleteUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => _success);

            _classUnderTest = new DeleteUserHandler(_trackLoginService.Object);
        }

        [Test]
        public void Success()
        {
            string postcode = "POSTCODE";
            int userId = 1;
            CancellationToken cancellationToken = CancellationToken.None;
            _success = true;

            var result = _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.DeleteUserRequest()
            {
                Postcode = postcode,
                UserID = userId,
            }, cancellationToken).Result;

            Assert.AreEqual(_success, result.Success);
            _trackLoginService.Verify(x => x.DeleteUser(userId, postcode, cancellationToken), Times.Once);
        }
    }

}
