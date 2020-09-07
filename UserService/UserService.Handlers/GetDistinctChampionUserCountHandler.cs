using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class GetDistinctChampionUserCountHandler : IRequestHandler<GetDistinctChampionUserCountRequest, GetDistinctChampionUserCountResponse>
    {
        private readonly IRepository _repository;

        public GetDistinctChampionUserCountHandler(IRepository repository)
        {
            _repository = repository;
        }

        [Transaction(Web = true)]
        public Task<GetDistinctChampionUserCountResponse> Handle(GetDistinctChampionUserCountRequest request, CancellationToken cancellationToken)
        {
            var count = _repository.GetDistinctChampionUserCount();

            var response = new GetDistinctChampionUserCountResponse()
            {
                Count = count
            };
            
            return Task.FromResult(response);
        }
    }
}
