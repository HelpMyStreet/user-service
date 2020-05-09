using AzureFunctions.Extensions.Swashbuckle.Attribute;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UserService.Core.Domains.Entities;

namespace UserService.AzureFunction
{
    public class GetNumberOfVolunteersInBoundary
    {
        private readonly IMediator _mediator;

        public GetNumberOfVolunteersInBoundary(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetNumberOfVolunteersInBoundary")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetNumberOfVolunteersInBoundaryResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetNumberOfVolunteersInBoundaryRequest), "product request")] GetNumberOfVolunteersInBoundaryRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetNumberOfVolunteersInBoundary");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetNumberOfVolunteersInBoundaryResponse response = await _mediator.Send(req);

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