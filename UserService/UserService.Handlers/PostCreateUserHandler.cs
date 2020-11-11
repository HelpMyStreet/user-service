using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class PostCreateUserHandler : IRequestHandler<PostCreateUserRequest, PostCreateUserResponse>
    {
        private readonly IRepository _repository;

        public PostCreateUserHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PostCreateUserResponse> Handle(PostCreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = _repository.GetUserByFirebaseUserID(request.RegistrationStepOne.FirebaseUID);
            int userId;

            if(user!=null)
            {
                userId = user.ID;
            }
            else
            {
                userId = _repository.PostCreateUser(request.RegistrationStepOne.FirebaseUID, request.RegistrationStepOne.EmailAddress, request.RegistrationStepOne.DateCreated, request.RegistrationStepOne.ReferringGroupId, request.RegistrationStepOne.Source);
            }

            return Task.FromResult(new PostCreateUserResponse() { ID = userId });
        }
    }
}
