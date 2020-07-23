using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;

namespace UserService.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> DeleteUser(string firebaseUserID);
    }
}