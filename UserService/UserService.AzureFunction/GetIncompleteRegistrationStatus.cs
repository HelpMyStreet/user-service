using AzureFunctions.Extensions.Swashbuckle.Attribute;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;

namespace UserService.AzureFunction
{
    public class GetIncompleteRegistrationStatus
    {
        private readonly IMediator _mediator;

        public GetIncompleteRegistrationStatus(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetIncompleteRegistrationStatus")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetIncompleteRegistrationStatusResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetIncompleteRegistrationStatusRequest), "product request")] GetIncompleteRegistrationStatusRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetIncompleteRegistrationStatus");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetIncompleteRegistrationStatusResponse response = await _mediator.Send(req);

                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(exc)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
