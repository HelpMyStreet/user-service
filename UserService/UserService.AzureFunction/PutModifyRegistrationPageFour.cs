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
    public class PutModifyRegistrationPageFour
    {
        private readonly IMediator _mediator;

        public PutModifyRegistrationPageFour(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PutModifyRegistrationPageFour")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PutModifyRegistrationPageFourResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]
            [RequestBodyType(typeof(PutModifyRegistrationPageFourRequest), "product request")] PutModifyRegistrationPageFourRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PutModifyRegistrationPageFour");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PutModifyRegistrationPageFourResponse response = await _mediator.Send(req);
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
