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
using HelpMyStreet.Contracts.GroupService.Request;
using Microsoft.Extensions.Internal;

namespace UserService.Core.Services
{
    public class TrackLoginService : ITrackLoginService
    {
        private readonly ISystemClock _systemClock;
        private readonly IAuthService _authService;
        private readonly ICommunicationService _communicationService;
        private readonly IRepository _repository;
        private readonly IGroupService _groupService;

        public TrackLoginService(ISystemClock systemClock, IAuthService authService, ICommunicationService communicationService, IRepository repository, IGroupService groupService)
        {
            _systemClock = systemClock;
            _authService = authService;
            _communicationService = communicationService;
            _repository = repository;
            _groupService = groupService;         
        }

        public async Task CheckLogins()
        {
            DateTime dtChecked = _systemClock.UtcNow.DateTime;

            var users = await _repository.GetAllUsers();

            var tasks = users.ChunkBy(100).Select(async (chunk) =>
            {
                return await _authService.GetHistoryForUsers(chunk.Select(x => x.FirebaseUID).ToList());
            });

            List<UserHistory> history = (await Task.WhenAll(tasks)).SelectMany(rss => rss).ToList();

            await _repository.UpdateLoginChecks(dtChecked, history);

            await ManageInactiveUsers(2);
        }

        public async Task ManageInactiveUsers(int yearsInActive)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            var inactiveUsers =  await _repository.GetInactiveUsers(yearsInActive);

            inactiveUsers.ForEach(async user =>
            {
                var dateLastEmailSent = await _communicationService.GetDateEmailLastSentAsync(
                    new GetDateEmailLastSentRequest()
                    {
                        RecipientUserId = user.UserId,
                        TemplateName = "ImpendingUserDeletion"
                    }, cancellationToken);

                if(!dateLastEmailSent.HasValue)
                {
                    await _communicationService.RequestCommunicationAsync(new RequestCommunicationRequest()
                    {
                        CommunicationJob = new CommunicationJob()
                        {
                            CommunicationJobType = CommunicationJobTypes.ImpendingUserDeletion,
                        },
                        RecipientUserID = user.UserId,
                        AdditionalParameters = new Dictionary<string, string>()
                    {
                        { "LastActiveDate", user.DateLastLogin.Value.FriendlyPastDate() }
                    }
                    }, CancellationToken.None);
                }
                else if ((_systemClock.UtcNow - dateLastEmailSent.Value).TotalDays >= 30)
                {
                    //Delete the user
                    await DeleteUser(user.UserId, user.Postcode, cancellationToken);
                }
            });
        }

        public async Task<bool> DeleteUser(int userId, string postcode, CancellationToken cancellationToken)
        {
            bool success = false;
            int groupId;
            if (userId > 0)
            {
                var result = _repository.GetUserByID(userId);

                if (result != null && result.PostalCode == postcode)
                {
                    success = _authService.DeleteUser(result.FirebaseUID).Result;

                    if (success)
                    {
                        var groupRoles = await _groupService.GetUserRoles(userId, cancellationToken);

                        foreach (KeyValuePair<int, List<int>> group in groupRoles)
                        {
                            groupId = group.Key;
                            foreach (int role in group.Value.OrderByDescending(x => x))
                            {
                                await _groupService.PostRevokeRole(new PostRevokeRoleRequest()
                                {
                                    GroupID = group.Key,
                                    AuthorisedByUserID = -1,
                                    UserID = userId,
                                    Role = new RoleRequest()
                                    {
                                        GroupRole = (HelpMyStreet.Utils.Enums.GroupRoles)role
                                    }
                                }, cancellationToken);
                            }
                        }

                        success = await _repository.DeleteUserAsync(userId, cancellationToken);
                        bool deletedMarketingContact = await _communicationService.DeleteMarketingContactAsync(new DeleteMarketingContactRequest()
                        {
                            EmailAddress = result.UserPersonalDetails.EmailAddress
                        }, cancellationToken);

                        if (!deletedMarketingContact)
                        {
                            throw new Exception("Unable to delete email address from marketing list");
                        }
                    }
                }
            }
            return success;
        }

    }
}
