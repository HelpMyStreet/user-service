using System;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.CommunicationService.Response;
using HelpMyStreet.Contracts.RequestService.Response;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using UserService.Core.Interfaces.Services;

namespace UserService.AzureFunction
{
    public class RegistrationChasers
    {
        private readonly ICommunicationService _communicationService;

        public RegistrationChasers(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [FunctionName("RegistrationChasers")]
        public async Task Run([TimerTrigger("%RegistrationChasersCronExpression%")]TimerInfo myTimer, ILogger log)
        {
            RequestCommunicationRequest req = new RequestCommunicationRequest()
            {
                CommunicationJob = new CommunicationJob()
                {
                    CommunicationJobType = CommunicationJobTypes.SendRegistrationChasers
                }
            };

            await _communicationService.RequestCommunicationAsync(req, CancellationToken.None);
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
