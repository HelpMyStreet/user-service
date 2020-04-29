using UserService.Core.Domains.Entities;
using UserService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NewRelic.Api.Agent;

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
