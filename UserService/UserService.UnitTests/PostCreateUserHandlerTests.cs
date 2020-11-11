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
    public class PostCreateUserHandlerTests
    {
        private PostCreateUserHandler _classUnderTest;
        private Mock<IRepository> _repository;
        private User _user;
        private int _userId;

        [SetUp]
        public void SetUp()
        {
            SetupRepository();
            _classUnderTest = new PostCreateUserHandler(_repository.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetUserByFirebaseUserID(It.IsAny<string>()))
                .Returns(() => _user);

            _repository.Setup(x => x.PostCreateUser(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime?>(), 
                It.IsAny<int?>(), 
                It.IsAny<string>()))
                .Returns(() => _userId);
        }

        [Test]
        public void WhenFirebaseAlreadyExistsReturn_ExistingID()
        {
            _user = new User()
            {
                PostalCode = "POSTCODE",
                UserPersonalDetails = new UserPersonalDetails()
                {
                    EmailAddress = "EMAILADDRESS"
                },
                ID  = 1
            };
           
            var result = _classUnderTest.Handle(new PostCreateUserRequest()
            {
                RegistrationStepOne = new RegistrationStepOne()
                {
                    FirebaseUID = "FirebaseUID",
                    EmailAddress = "EmailAddress"
                }
            }, CancellationToken.None).Result;

            Assert.AreEqual(result.ID, _user.ID);
            _repository.Verify(x => x.GetUserByFirebaseUserID(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.PostCreateUser(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int?>(),
                It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void WhenUserNotFound_CreateNewUser()
        {
            _user = null;
            _userId = 1;
            var result = _classUnderTest.Handle(new PostCreateUserRequest()
            {
                RegistrationStepOne = new RegistrationStepOne()
                {
                    FirebaseUID = "FirebaseUID",
                    EmailAddress = "EmailAddress"
                }
            }, CancellationToken.None).Result;

            Assert.AreEqual(result.ID, _userId );
            _repository.Verify(x => x.GetUserByFirebaseUserID(It.IsAny<string>()), Times.Once);
            _repository.Verify(x => x.PostCreateUser(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int?>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}
