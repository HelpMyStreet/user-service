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
    public class PutModifyRegistrationPageFiveHandler : IRequestHandler<PutModifyRegistrationPageFiveRequest, PutModifyRegistrationPageFiveResponse>
    {
        private readonly IRepository _repository;

        public PutModifyRegistrationPageFiveHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutModifyRegistrationPageFiveResponse> Handle(PutModifyRegistrationPageFiveRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUserRegistrationPageFive(request.RegistrationStepFive);

            return Task.FromResult(new PutModifyRegistrationPageFiveResponse() { ID = response});
        }
    }
}
