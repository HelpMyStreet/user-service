using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
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
