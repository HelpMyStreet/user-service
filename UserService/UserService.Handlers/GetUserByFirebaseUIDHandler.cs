using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

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
