using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.CommunicationService.Request;
using UserService.Core.Exceptions;
using System.Net.Http;

namespace UserService.Handlers
{
    public class PutModifyRegistrationPageTwoHandler : IRequestHandler<PutModifyRegistrationPageTwoRequest, PutModifyRegistrationPageTwoResponse>
    {
        private readonly IRepository _repository;
        private readonly ICommunicationService _communicationService;
        private readonly IAddressService _addressService;

        public PutModifyRegistrationPageTwoHandler(IRepository repository, ICommunicationService communicationService, IAddressService addressService)
        {
            _repository = repository;
            _communicationService = communicationService;
            _addressService = addressService;
        }

        public async Task<PutModifyRegistrationPageTwoResponse> Handle(PutModifyRegistrationPageTwoRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.RegistrationStepTwo.PostalCode))
            {
                request.RegistrationStepTwo.PostalCode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(request.RegistrationStepTwo.PostalCode);

                try
                {
                    var postcodeValid = await _addressService.IsValidPostcode(request.RegistrationStepTwo.PostalCode, cancellationToken);
                }
                catch (HttpRequestException)
                {
                    throw new PostCodeException();
                }
            }

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
