using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Contracts;

namespace UserService.Core.Interfaces.Services
{
    public interface ITrackLoginService
    {
        Task CheckLogins();

        Task ManageInactiveUsers(int yearsInActive);

        Task<bool> DeleteUser(int userId, string postcode, bool checkPostcode, CancellationToken cancellationToken);
    }
}