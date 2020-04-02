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
    public class GetVolunteerCountByPostcodeHandler : IRequestHandler<GetVolunteerCountByPostcodeRequest, GetVolunteerCountByPostcodeResponse>
    {
        private readonly IRepository _repository;

        public GetVolunteerCountByPostcodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetVolunteerCountByPostcodeResponse> Handle(GetVolunteerCountByPostcodeRequest request, CancellationToken cancellationToken)
        {
            var count = _repository.GetVolunteerCountByPostCode(request.PostCode);

            var response = new GetVolunteerCountByPostcodeResponse()
            {
                Count = count
            };
            
            return Task.FromResult(response);
        }
    }
}
