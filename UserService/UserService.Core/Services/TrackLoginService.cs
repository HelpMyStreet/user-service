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
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
using System.Threading;

namespace UserService.Core.Services
{
    public class TrackLoginService : ITrackLoginService
    {
        private readonly IAuthService _authService;
        private readonly ICommunicationService _communicationService;
        private readonly IRepository _repository;

        public TrackLoginService(IAuthService authService, ICommunicationService communicationService, IRepository repository)
        {
            _authService = authService;
            _communicationService = communicationService;
            _repository = repository;
        }

        public async Task CheckLogins()
        {
            DateTime dtChecked = DateTime.UtcNow;

            var users = await _repository.GetAllUsers();

            var tasks = users.ChunkBy(100).Select(async (chunk) =>
            {
                return await _authService.GetHistoryForUsers(chunk.Select(x => x.FirebaseUID).ToList());
            });

            List<UserHistory> history = (await Task.WhenAll(tasks)).SelectMany(rss => rss).ToList();

            await _repository.UpdateLoginChecks(dtChecked, history);

            await NotifyInactiveUsers(2);
        }

        public async Task NotifyInactiveUsers(int yearsInActive)
        {
            var inactiveUsers =  await _repository.GetInactiveUsers(yearsInActive);

            inactiveUsers.ForEach(user =>
            {
                _communicationService.RequestCommunicationAsync(new RequestCommunicationRequest()
                {
                    CommunicationJob = new CommunicationJob()
                    {
                        CommunicationJobType = CommunicationJobTypes.ImpendingUserDeletion,
                    },
                    RecipientUserID = user.Item1,
                    AdditionalParameters = new Dictionary<string, string>()
                    {
                        { "LastActiveDate", user.Item2.Value.FriendlyPastDate() }
                    }
                }, CancellationToken.None);
            });
        }
    }
}
