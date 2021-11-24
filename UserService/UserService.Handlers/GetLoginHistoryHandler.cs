using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using UserService.Core.Interfaces.Services;
using System;
using HelpMyStreet.Contracts.CommunicationService.Request;
using System.Collections.Generic;
using HelpMyStreet.Contracts.GroupService.Request;
using System.Linq;
using UserService.Core.Contracts;
using HelpMyStreet.Utils.Extensions;

namespace UserService.Handlers
{
    public class GetLoginHistoryHandler : IRequestHandler<GetLoginHistoryRequest, GetLoginHistoryResponse>
    {
        private readonly IRepository _repository;
        private readonly IAuthService _authService;

        public GetLoginHistoryHandler(IRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<GetLoginHistoryResponse> Handle(GetLoginHistoryRequest request, CancellationToken cancellationToken)
        {
            GetLoginHistoryResponse response = new GetLoginHistoryResponse()
            {
                History = new List<UserHistory>()
            };

            var users = await _repository.GetAllUsers(cancellationToken);

            var tasks = users.ChunkBy(100).Select(async (chunk) =>
            {
                return  await _authService.GetHistoryForUsers(chunk.Select(x => x.FirebaseUID).ToList());
            });

            response.History = (await Task.WhenAll(tasks)).SelectMany(rss => rss).ToList();

            return response;
        }
    }
}
