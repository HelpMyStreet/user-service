using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class GetChampionPostcodesCoveredCountHandler : IRequestHandler<GetChampionPostcodesCoveredCountRequest, GetChampionPostcodesCoveredCountResponse>
    {
        private readonly IRepository _repository;

        public GetChampionPostcodesCoveredCountHandler(IRepository repository)
        {
            _repository = repository;
        }

        [Transaction(Web = true)]
        public Task<GetChampionPostcodesCoveredCountResponse> Handle(GetChampionPostcodesCoveredCountRequest request, CancellationToken cancellationToken)
        {
            var count = _repository.GetChampionPostcodesCoveredCount();

            var response = new GetChampionPostcodesCoveredCountResponse()
            {
                Count = count
            };
            
            return Task.FromResult(response);
        }
    }
}
