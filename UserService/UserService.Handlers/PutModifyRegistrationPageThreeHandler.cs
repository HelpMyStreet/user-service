using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.RequestService.Response;

namespace UserService.Handlers
{
    public class PutModifyRegistrationPageThreeHandler : IRequestHandler<PutModifyRegistrationPageThreeRequest, PutModifyRegistrationPageThreeResponse>
    {
        private readonly IRepository _repository;
        private readonly ICommunicationService _communicationService;

        public PutModifyRegistrationPageThreeHandler(IRepository repository, ICommunicationService communicationService)
        {
            _repository = repository;
            _communicationService = communicationService;
        }

        public Task<PutModifyRegistrationPageThreeResponse> Handle(PutModifyRegistrationPageThreeRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUserRegistrationPageThree(request.RegistrationStepThree);

            if(response>0)
            {
                _communicationService.RequestCommunicationAsync(new RequestCommunicationRequest()
                {
                    CommunicationJob = new CommunicationJob()
                    {
                        CommunicationJobType = CommunicationJobTypes.PostYotiCommunication,
                    },
                    RecipientUserID = response
                }, cancellationToken);
            }

            return Task.FromResult(new PutModifyRegistrationPageThreeResponse() { ID = response});
        }
    }
}
