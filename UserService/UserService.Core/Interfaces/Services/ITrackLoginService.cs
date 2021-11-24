using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Core.Contracts;

namespace UserService.Core.Interfaces.Services
{
    public interface ITrackLoginService
    {
        Task CheckLogins();
    }
}