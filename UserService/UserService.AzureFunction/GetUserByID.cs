using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;
using HelpMyStreet.Utils.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UserService.AzureFunction
{
    public class GetUserByID
    {
        private readonly IMediator _mediator;

        public GetUserByID(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetUserByID")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetUserByIDRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                List<HelpMyStreet.Utils.Enums.SupportActivities> supportActivities = new System.Collections.Generic.List<HelpMyStreet.Utils.Enums.SupportActivities>();
                supportActivities.Add(HelpMyStreet.Utils.Enums.SupportActivities.Errands);
                supportActivities.Add(HelpMyStreet.Utils.Enums.SupportActivities.DogWalking);

                RegistrationStepThree registrationStepThree = new RegistrationStepThree()
                {
                    Activities = supportActivities
                };

                string jsonData = JsonConvert.SerializeObject(registrationStepThree);

                GetUserByIDResponse response = await _mediator.Send(req);
                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
