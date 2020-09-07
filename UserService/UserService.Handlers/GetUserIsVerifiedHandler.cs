using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class GetUserIsVerifiedHandler : IRequestHandler<GetUserIsVerifiedRequest, GetUserIsVerifiedResponse>
    {
        private readonly IRepository _repository;

        public GetUserIsVerifiedHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<GetUserIsVerifiedResponse> Handle(GetUserIsVerifiedRequest request, CancellationToken cancellationToken)
        {
            bool result = _repository.GetUserIsVerified(request.UserID);
            return Task.FromResult(new GetUserIsVerifiedResponse()
            {
                IsVerified = result
            }) ;
        }
    }
}
