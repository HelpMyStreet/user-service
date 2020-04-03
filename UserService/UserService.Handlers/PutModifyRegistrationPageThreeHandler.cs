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
    public class PutModifyRegistrationPageThreeHandler : IRequestHandler<PutModifyRegistrationPageThreeRequest, PutModifyRegistrationPageThreeResponse>
    {
        private readonly IRepository _repository;

        public PutModifyRegistrationPageThreeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutModifyRegistrationPageThreeResponse> Handle(PutModifyRegistrationPageThreeRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUserRegistrationPageThree(request.RegistrationStepThree);

            return Task.FromResult(new PutModifyRegistrationPageThreeResponse() { ID = response});
        }
    }
}
