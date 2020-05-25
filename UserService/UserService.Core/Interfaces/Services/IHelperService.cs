using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;

namespace UserService.Core.Interfaces.Services
{
    public interface IHelperService
    {
        Task<IEnumerable<User>> GetHelpersWithinRadius(string postcode, CancellationToken token);
    }
}