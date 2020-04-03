using UserService.Core.Domains.Entities;
using UserService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Handlers
{
    public class GetDistinctChampionUserCountHandler : IRequestHandler<GetDistinctChampionUserCountRequest, GetDistinctChampionUserCountResponse>
    {
        private readonly IRepository _repository;

        public GetDistinctChampionUserCountHandler(IRepository repository)
        {
            _repository = repository;
        }

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
