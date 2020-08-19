using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Request;

namespace UserService.AzureFunction
{
    public class PostCreateSupportForPostCode
    {
        private readonly IMediator _mediator;

        public PostCreateSupportForPostCode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostCreateSupportForPostCode")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(PostCreateSupportForPostCodeRequest), "product request")] PostCreateSupportForPostCodeRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PostCreateSupportForPostCode");
                log.LogInformation("C# HTTP trigger function processed a request.");

                await _mediator.Send(req);
                return new NoContentResult();
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
