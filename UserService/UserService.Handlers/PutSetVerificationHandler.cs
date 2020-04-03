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
    public class PutSetVerificationHandler : IRequestHandler<PutSetVerificationRequest, PutSetVerificationResponse>
    {
        private readonly IRepository _repository;

        public PutSetVerificationHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutSetVerificationResponse> Handle(PutSetVerificationRequest request, CancellationToken cancellationToken)
        {
            bool response = _repository.SetUserVerfication(request.UserID,request.IsVerified);

            return Task.FromResult(new PutSetVerificationResponse() {Success = response });
        }
    }
}
