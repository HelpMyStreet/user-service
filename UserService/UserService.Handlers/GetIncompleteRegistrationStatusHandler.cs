using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class GetIncompleteRegistrationStatusHandler : IRequestHandler<GetIncompleteRegistrationStatusRequest, GetIncompleteRegistrationStatusResponse>
    {
        private readonly IRepository _repository;

        public GetIncompleteRegistrationStatusHandler(IRepository repository)
        {
            _repository = repository;
        }

        [Transaction(Web = true)]
        public async Task<GetIncompleteRegistrationStatusResponse> Handle(GetIncompleteRegistrationStatusRequest request, CancellationToken cancellationToken)
        {
            List<UserRegistrationStep> users = await _repository.GetIncompleteRegistrationStatusAsync(cancellationToken);

            var response = new GetIncompleteRegistrationStatusResponse()
            {
                Users = users
            };
            
            return response;
        }
    }
}
