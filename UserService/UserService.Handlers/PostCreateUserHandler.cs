using UserService.Core.Domains.Entities;
using UserService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            int userId = _repository.PostCreateUser(request.RegistrationStepOne.FirebaseUID,request.RegistrationStepOne.EmailAddress, request.RegistrationStepOne.DateCreated);

            return Task.FromResult(new PostCreateUserResponse() { ID = userId });
        }
    }
}
