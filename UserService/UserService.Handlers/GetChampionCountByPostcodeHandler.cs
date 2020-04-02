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
    public class GetChampionCountByPostcodeHandler : IRequestHandler<GetChampionCountByPostcodeRequest, GetChampionCountByPostcodeResponse>
    {
        private readonly IRepository _repository;

        public GetChampionCountByPostcodeHandler(IRepository repository)
        {
            _repository = repository;
        }

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
