using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<List<SupportActivityConfiguration>> GetSupportActivitiesConfigurationAsync(CancellationToken cancellationToken);
    }
}