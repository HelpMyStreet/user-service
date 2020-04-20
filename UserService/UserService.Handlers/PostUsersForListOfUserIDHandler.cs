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
    public class PostUsersForListOfUserIDHandler : IRequestHandler<PostUsersForListOfUserIDRequest, PostUsersForListOfUserIDResponse>
    {
        private readonly IRepository _repository;

        public PostUsersForListOfUserIDHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<PostUsersForListOfUserIDResponse> Handle(PostUsersForListOfUserIDRequest request, CancellationToken cancellationToken)
        {
            var users = _repository.GetUsersForIDs(request.ListUserID.UserIDs);

            return Task.FromResult(new PostUsersForListOfUserIDResponse()
            {
                Users = users
            });
        }
    }
}
