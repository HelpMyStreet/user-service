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
    public class GetChampionPostcodesCoveredCountHandler : IRequestHandler<GetChampionPostcodesCoveredCountRequest, GetChampionPostcodesCoveredCountResponse>
    {
        private readonly IRepository _repository;

        public GetChampionPostcodesCoveredCountHandler(IRepository repository)
        {
            _repository = repository;
        }

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
