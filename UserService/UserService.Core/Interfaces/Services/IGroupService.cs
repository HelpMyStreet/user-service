using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Contracts.GroupService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<List<SupportActivityConfiguration>> GetSupportActivitiesConfigurationAsync(CancellationToken cancellationToken);

        Task<List<SupportActivityDetail>> GetRegistrationFormSupportActivities(RegistrationFormVariant registrationFormVariant, CancellationToken cancellationToken);

        Task<Dictionary<int, List<int>>> GetUserRoles(int userId, CancellationToken cancellationToken);

        Task<GroupPermissionOutcome> PostRevokeRole(PostRevokeRoleRequest request, CancellationToken cancellationToken);

    }
}