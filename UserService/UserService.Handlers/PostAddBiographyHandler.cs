using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.Handlers
{
    public class PostAddBiographyHandler : IRequestHandler<PostAddBiographyRequest, PostAddBiographyResponse>
    {
        private readonly IRepository _repository;

        public PostAddBiographyHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<PostAddBiographyResponse> Handle(PostAddBiographyRequest request, CancellationToken cancellationToken)
        {
            var result = new PostAddBiographyResponse()
            {
                Outcome = HelpMyStreet.Utils.Enums.UpdateBiographyOutcome.BadRequest
            };

            var user = _repository.GetUserByID(request.UserId);

            if (user != null)
            {
                var success = _repository.AddBiography(request.UserId, request.Details);
                if(success)
                {
                    result.Outcome = HelpMyStreet.Utils.Enums.UpdateBiographyOutcome.Success;
                }
            }

            return result;
        }
    }
}
