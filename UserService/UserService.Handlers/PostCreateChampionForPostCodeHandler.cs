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
