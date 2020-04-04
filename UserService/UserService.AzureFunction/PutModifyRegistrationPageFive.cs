using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace UserService.AzureFunction
{
    public class PutModifyRegistrationPageFive
    {
        private readonly IMediator _mediator;

        public PutModifyRegistrationPageFive(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PutModifyRegistrationPageFive")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PutModifyRegistrationPageFiveResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]
            [RequestBodyType(typeof(PutModifyRegistrationPageFiveRequest), "product request")] PutModifyRegistrationPageFiveRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                PutModifyRegistrationPageFiveResponse response = await _mediator.Send(req);
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
