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
    public class GetChampionsByPostcodeHandler : IRequestHandler<GetChampionsByPostcodeRequest, GetChampionsByPostcodeResponse>
    {
        private readonly IRepository _repository;

        public GetChampionsByPostcodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetChampionsByPostcodeResponse> Handle(GetChampionsByPostcodeRequest request, CancellationToken cancellationToken)
        {
            List<HelpMyStreet.Utils.Models.User> result = _repository.GetChampionsByPostCode(request.PostCode);

            var response = new GetChampionsByPostcodeResponse()
            {
                Users = new List<HelpMyStreet.Utils.Models.User>()
            };

            foreach (HelpMyStreet.Utils.Models.User user in result)
            {
                response.Users.Add(user);
            }
            
            return Task.FromResult(response);
        }
    }
}
