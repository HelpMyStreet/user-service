using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class GetDistinctVolunteerUserCountHandler : IRequestHandler<GetDistinctVolunteerUserCountRequest, GetDistinctVolunteerUserCountResponse>
    {
        private readonly IRepository _repository;

        public GetDistinctVolunteerUserCountHandler(IRepository repository)
        {
            _repository = repository;
        }

        [Transaction(Web = true)]
        public Task<GetDistinctVolunteerUserCountResponse> Handle(GetDistinctVolunteerUserCountRequest request, CancellationToken cancellationToken)
        {
            var count = _repository.GetDistinctVolunteerUserCount();

            var response = new GetDistinctVolunteerUserCountResponse()
            {
                Count = count
            };
            
            return Task.FromResult(response);
        }
    }
}
