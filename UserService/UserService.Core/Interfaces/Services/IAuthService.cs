using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> DeleteUser(string firebaseUserID);
    }
}