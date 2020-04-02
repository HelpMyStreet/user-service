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
    public class GetUserIsVerifiedHandler : IRequestHandler<GetUserIsVerifiedRequest, GetUserIsVerifiedResponse>
    {
        private readonly IRepository _repository;

        public GetUserIsVerifiedHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetUserIsVerifiedResponse> Handle(GetUserIsVerifiedRequest request, CancellationToken cancellationToken)
        {
            bool result = _repository.GetUserIsVerified(request.ID);
            return Task.FromResult(new GetUserIsVerifiedResponse()
            {
                IsVerified = result
            }) ;
        }
    }
}
