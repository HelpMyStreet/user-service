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
    public class PutModifyRegistrationPageFourHandler : IRequestHandler<PutModifyRegistrationPageFourRequest, PutModifyRegistrationPageFourResponse>
    {
        private readonly IRepository _repository;

        public PutModifyRegistrationPageFourHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutModifyRegistrationPageFourResponse> Handle(PutModifyRegistrationPageFourRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUserRegistrationPageFour(request.RegistrationStepFour);

            return Task.FromResult(new PutModifyRegistrationPageFourResponse() { ID = response});
        }
    }
}
