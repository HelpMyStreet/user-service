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
    public class PutModifyUserHandler : IRequestHandler<PutModifyUserRequest, PutModifyUserResponse>
    {
        private readonly IRepository _repository;

        public PutModifyUserHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutModifyUserResponse> Handle(PutModifyUserRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUser(request.User);

            return Task.FromResult(new PutModifyUserResponse() {UserID = response });
        }
    }
}
