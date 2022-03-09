using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using UserService.Handlers;

namespace UserService.UnitTests
{
    public class PostAddBiographyHandlerTests
    {
        private PostAddBiographyHandler _classUnderTest;
        private Mock<IRepository> _repository;
        private User _user;        
        private bool _success;


        [SetUp]
        public void SetUp()
        {
            SetupRepository();            
            _classUnderTest = new PostAddBiographyHandler(_repository.Object) ;
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetUserByID(It.IsAny<int>()))
                .Returns(() => _user);
            _repository.Setup(x => x.AddBiography(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(() => _success);            
        }

        [Test]
        public async Task Success()
        {
            int userId = -1;
            _user = new User()
            {
                ID = userId
            };
            _success = true;

            var result = await _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.PostAddBiographyRequest()
            {
                UserId = userId,
                Details = "Test"
            },CancellationToken.None);

            Assert.AreEqual(result.Outcome, UpdateBiographyOutcome.Success);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()),Times.Once);
            _repository.Verify(x => x.AddBiography(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task WhenUserNotFound_ReturnsBadRequest()
        {
            int userId = -1;
            _user = null;
            _success = false;

            var result = await _classUnderTest.Handle(new HelpMyStreet.Contracts.UserService.Request.PostAddBiographyRequest()
            {
                UserId = userId,
                Details = "Test"
            }, CancellationToken.None);

            Assert.AreEqual(result.Outcome, UpdateBiographyOutcome.BadRequest);
            _repository.Verify(x => x.GetUserByID(It.IsAny<int>()), Times.Once);
            _repository.Verify(x => x.AddBiography(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }        
    }
}
