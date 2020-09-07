using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class GetChampionCountByPostcodeHandler : IRequestHandler<GetChampionCountByPostcodeRequest, GetChampionCountByPostcodeResponse>
    {
        private readonly IRepository _repository;

        public GetChampionCountByPostcodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        [Transaction(Web = true)]
        public Task<GetChampionCountByPostcodeResponse> Handle(GetChampionCountByPostcodeRequest request, CancellationToken cancellationToken)
        {
            var count = _repository.GetChampionCountByPostCode(request.PostCode);

            var response = new GetChampionCountByPostcodeResponse()
            {
                Count = count
            };
            
            return Task.FromResult(response);
        }
    }
}
