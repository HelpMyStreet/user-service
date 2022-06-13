using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using UserService.Core.Interfaces.Services;

namespace UserService.AzureFunction
{
    public class TrackLogins
    {
        private readonly ITrackLoginService _trackLoginService;

        public TrackLogins(ITrackLoginService trackLoginService)
        {
            _trackLoginService = trackLoginService;
        }
        [FunctionName("TrackLogins")]
        public async Task Run([TimerTrigger("%TrackLoginsCronExpression%")]TimerInfo myTimer, ILogger log)
        {
            await _trackLoginService.CheckLogins();
        }
    }
}
