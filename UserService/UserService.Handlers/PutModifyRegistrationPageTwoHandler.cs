using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class PutModifyRegistrationPageTwoHandler : IRequestHandler<PutModifyRegistrationPageTwoRequest, PutModifyRegistrationPageTwoResponse>
    {
        private readonly IRepository _repository;

        public PutModifyRegistrationPageTwoHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutModifyRegistrationPageTwoResponse> Handle(PutModifyRegistrationPageTwoRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUserRegistrationPageTwo(request.RegistrationStepTwo);

            return Task.FromResult(new PutModifyRegistrationPageTwoResponse() { ID = response});
        }
    }
}
