using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class PutModifyUserHandler : IRequestHandler<PutModifyUserRequest, PutModifyUserResponse>
    {
        private readonly IRepository _repository;

        public PutModifyUserHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PutModifyUserResponse> Handle(PutModifyUserRequest request, CancellationToken cancellationToken)
        {
            int response = _repository.ModifyUser(request.User);

            return Task.FromResult(new PutModifyUserResponse() {UserID = response });
        }
    }
}
