using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

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
