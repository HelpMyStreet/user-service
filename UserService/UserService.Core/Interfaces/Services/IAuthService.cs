using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Core.Contracts;

namespace UserService.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> DeleteUser(string firebaseUserID);
        Task<List<UserHistory>> GetHistoryForUsers(List<string> firebaseUserIds);
    }
}