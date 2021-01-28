using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;

namespace UserService.Core.Interfaces.Services
{
    public interface IHelperService
    {
        Task<IEnumerable<HelperWithinRadiusDTO>> GetHelpersWithinRadius(string postcode, double? overrideVolunteerRadius, CancellationToken token);
    }
}