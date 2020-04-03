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
    public class GetUserByFirebaseUIDHandler : IRequestHandler<GetUserByFirebaseUIDRequest, GetUserByFirebaseUIDResponse>
    {
        private readonly IRepository _repository;

        public GetUserByFirebaseUIDHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetUserByFirebaseUIDResponse> Handle(GetUserByFirebaseUIDRequest request, CancellationToken cancellationToken)
        {
            var user = _repository.GetUserByFirebaseUserID(request.FirebaseUID);

            return Task.FromResult(new GetUserByFirebaseUIDResponse()
            {
                User = user
            });
        }
    }
}
