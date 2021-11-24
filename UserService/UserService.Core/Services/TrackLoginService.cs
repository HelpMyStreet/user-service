using System;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using UserService.Core.Contracts;
using System.Collections.Generic;
using System.Linq;
using UserService.Core.Interfaces.Repositories;
using HelpMyStreet.Utils.Extensions;

namespace UserService.Core.Services
{
    public class TrackLoginService : ITrackLoginService
    {
        private readonly IAuthService _authService;
        private readonly IRepository _repository;

        public TrackLoginService(IAuthService authService, IRepository repository)
        {
            _authService = authService;
            _repository = repository;
        }

        public async Task CheckLogins()
        {
            var users = await _repository.GetAllUsers();

            var tasks = users.ChunkBy(100).Select(async (chunk) =>
            {
                return await _authService.GetHistoryForUsers(chunk.Select(x => x.FirebaseUID).ToList());
            });

            List<UserHistory> history = (await Task.WhenAll(tasks)).SelectMany(rss => rss).ToList();

            await _repository.UpdateLoginChecks(history);

            int i = 1;

        }
    }
}
