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
        private readonly IRepository _repository;

        public GetNewsTickerHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<NewsTickerResponse> Handle(NewsTickerRequest request, CancellationToken cancellationToken)
        {
            NewsTickerResponse response = new NewsTickerResponse()
            {
                Messages = new List<NewsTickerMessage>()
            };

            int activeUserCount = await _repository.GetActiveUserCount();

            if (activeUserCount >= 5)
            {
                response.Messages.Add(new NewsTickerMessage()
                {
                    Number = activeUserCount,
                    Message = $"**{activeUserCount}** volunteers waiting to help"
                });
            }

            return response;
        }
    }
}
