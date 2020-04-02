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
    public class GetUserByIDHandler : IRequestHandler<GetUserByIDRequest, GetUserByIDResponse>
    {
        private readonly IRepository _repository;

        public GetUserByIDHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetUserByIDResponse> Handle(GetUserByIDRequest request, CancellationToken cancellationToken)
        {
            var user = _repository.GetUserByID(request.ID);

            return Task.FromResult(new GetUserByIDResponse()
            {
                User = user
            });
        }
    }
}
