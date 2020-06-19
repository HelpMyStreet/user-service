using UserService.Core.Domains.Entities;
using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.CommunicationService.Request;

namespace UserService.Handlers
{
    public class PutSetVerificationHandler : IRequestHandler<PutSetVerificationRequest, PutSetVerificationResponse>
    {
        private readonly IRepository _repository;
        private readonly ICommunicationService _communicationService;

        public PutSetVerificationHandler(IRepository repository, ICommunicationService communicationService)
        {
            _repository = repository;
            _communicationService = communicationService;
        }

        public Task<PutSetVerificationResponse> Handle(PutSetVerificationRequest request, CancellationToken cancellationToken)
        {
            bool response = _repository.SetUserVerfication(request.UserID,request.IsVerified);
            _communicationService.RequestCommunicationAsync(new RequestCommunicationRequest()
            {
                CommunicationJob = new CommunicationJob()
                {
                    CommunicationJobType = CommunicationJobTypes.PostYotiCommunication,
                },
                RecipientUserID = request.UserID
            },cancellationToken);

            return Task.FromResult(new PutSetVerificationResponse() {Success = response });
        }
    }
}
