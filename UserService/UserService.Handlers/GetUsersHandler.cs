using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;

namespace UserService.Handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersRequest, GetUsersResponse>
    {
        private readonly IRepository _repository;

        public GetUsersHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            List<UserDetails> userDetails = await _repository.GetUserDetailsAsync(cancellationToken);

            return new GetUsersResponse()
            {
                UserDetails = userDetails
            };
        }
    }
}
