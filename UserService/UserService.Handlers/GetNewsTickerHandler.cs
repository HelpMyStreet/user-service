using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using HelpMyStreet.Contracts;

namespace UserService.Handlers
{
    public class GetNewsTickerHandler : IRequestHandler<NewsTickerRequest, NewsTickerResponse>
    {
        private readonly IRepository _repository;

        public GetNewsTickerHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<NewsTickerResponse> Handle(NewsTickerRequest request, CancellationToken cancellationToken)
        {

            return new NewsTickerResponse()
            {
                Messages = await _repository.GetNewsTickerMessages(request.GroupId, cancellationToken)
            };
        }
    }
}
