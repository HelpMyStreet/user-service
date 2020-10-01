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
        private readonly ICommunicationService _communicationService;

        public PutModifyRegistrationPageTwoHandler(IRepository repository, ICommunicationService communicationService)
        {
            _repository = repository;
            _communicationService = communicationService;
        }

        public async Task<PutModifyRegistrationPageTwoResponse> Handle(PutModifyRegistrationPageTwoRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUserRegistrationPageTwo(request.RegistrationStepTwo);

            var user =_repository.GetUserByID(response);

            if (user != null)
            {
                bool addedContact = await _communicationService.PutNewMarketingContactAsync(new PutNewMarketingContactRequest()
                {
                    MarketingContact = new MarketingContact()
                    {
                        FirstName = user.UserPersonalDetails.FirstName,
                        LastName = user.UserPersonalDetails.LastName,
                        EmailAddress = user.UserPersonalDetails.EmailAddress
                    }

                }, cancellationToken);

            }

            return new PutModifyRegistrationPageTwoResponse() { ID = response};
        }
    }
}
