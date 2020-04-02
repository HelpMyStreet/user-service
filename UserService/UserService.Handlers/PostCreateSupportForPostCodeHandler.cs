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
    public class PostCreateSupportForPostCodeHandler : IRequestHandler<PostCreateSupportForPostCodeRequest>
    {
        private readonly IRepository _repository;

        public PostCreateSupportForPostCodeHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<Unit> Handle(PostCreateSupportForPostCodeRequest request, CancellationToken cancellationToken)
        {
            _repository.CreateSupportForPostCode(request.UserID,request.PostCode);

            return Unit.Task;
        }
    }
}
