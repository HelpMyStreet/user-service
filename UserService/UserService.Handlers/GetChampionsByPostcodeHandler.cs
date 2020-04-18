using UserService.Core.Domains.Entities;
using UserService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;

namespace UserService.Handlers
{
    public class GetChampionsByPostcodeHandler : IRequestHandler<GetChampionsByPostcodeRequest, GetChampionsByPostcodeResponse>
    {
        private readonly IRepository _repository;

        public GetChampionsByPostcodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetChampionsByPostcodeResponse> Handle(GetChampionsByPostcodeRequest request, CancellationToken cancellationToken)
        {
            request.PostCode = PostcodeFormatter.FormatPostcode(request.PostCode);

            IReadOnlyList<HelpMyStreet.Utils.Models.User> result = await _repository.GetChampionsByPostCodeAsync(request.PostCode);

            GetChampionsByPostcodeResponse response = new GetChampionsByPostcodeResponse()
            {
                Users = result
            };
            
            return response;
        }
    }
}
