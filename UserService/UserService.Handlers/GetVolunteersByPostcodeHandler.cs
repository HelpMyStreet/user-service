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
    public class GetVolunteersByPostcodeHandler : IRequestHandler<GetVolunteersByPostcodeRequest, GetVolunteersByPostcodeResponse>
    {
        private readonly IRepository _repository;

        public GetVolunteersByPostcodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetVolunteersByPostcodeResponse> Handle(GetVolunteersByPostcodeRequest request, CancellationToken cancellationToken)
        {
            List<HelpMyStreet.Utils.Models.User> result = _repository.GetVolunteersByPostCode(request.PostCode);

            var response = new GetVolunteersByPostcodeResponse()
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
