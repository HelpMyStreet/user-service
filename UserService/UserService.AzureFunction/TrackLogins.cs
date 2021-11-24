using System;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.RequestService.Response;
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
        public async Task Run([TimerTrigger("%TrackLoginsCronExpression%", RunOnStartup =true)]TimerInfo myTimer, ILogger log)
        {
            await _trackLoginService.CheckLogins();
        }
    }
}
