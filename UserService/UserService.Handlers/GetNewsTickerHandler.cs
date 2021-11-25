using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using HelpMyStreet.Contracts;
using System.Collections.Generic;

namespace UserService.Handlers
{
    public class GetNewsTickerHandler : IRequestHandler<NewsTickerRequest, NewsTickerResponse>
    {
        public async Task<NewsTickerResponse> Handle(NewsTickerRequest request, CancellationToken cancellationToken)
        {
            NewsTickerResponse response = new NewsTickerResponse()
            {
                Messages = new List<NewsTickerMessage>()
            };

            return response;
        }
    }
}
