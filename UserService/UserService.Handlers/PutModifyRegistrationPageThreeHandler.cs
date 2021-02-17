using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using System.Linq;

namespace UserService.Handlers
{
    public class PutModifyRegistrationPageThreeHandler : IRequestHandler<PutModifyRegistrationPageThreeRequest, PutModifyRegistrationPageThreeResponse>
    {
        private readonly IRepository _repository;
        private readonly ICommunicationService _communicationService;
        private readonly IGroupService _groupService;

        public PutModifyRegistrationPageThreeHandler(IRepository repository, ICommunicationService communicationService, IGroupService groupService)
        {
            _repository = repository;
            _communicationService = communicationService;
            _groupService = groupService;
        }

        public Task<PutModifyRegistrationPageThreeResponse> Handle(PutModifyRegistrationPageThreeRequest request, CancellationToken cancellationToken)
        {
            if(request.RegistrationStepThree.Activities.Count>0 
                && request.RegistrationStepThree.Activities.Contains(HelpMyStreet.Utils.Enums.SupportActivities.Other))
            {
                var otherActivities = _groupService.GetSupportActivitiesConfigurationAsync(cancellationToken).Result;
                if(otherActivities!=null)
                {
                    var additionalActivities = otherActivities
                        .Where(x => !request.RegistrationStepThree.Activities.Contains(x.SupportActivity) && x.AutoSignUpWhenOtherSelected==true)
                        .Select(x=> x.SupportActivity)
                        .ToList();

                    if(additionalActivities!=null && additionalActivities.Count()>0)
                    {
                        request.RegistrationStepThree.Activities.AddRange(additionalActivities);
                    }
                }
            }

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
