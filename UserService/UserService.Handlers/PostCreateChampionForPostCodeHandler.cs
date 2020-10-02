using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;

namespace UserService.Handlers
{
    public class PostCreateChampionForPostCodeHandler : IRequestHandler<PostCreateChampionForPostCodeRequest>
    {
        private readonly IRepository _repository;

        public PostCreateChampionForPostCodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(PostCreateChampionForPostCodeRequest request, CancellationToken cancellationToken)
        {
            _repository.CreateChampionForPostCode(request.UserID,request.PostCode);

            return Unit.Task;
        }
    }
}
