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
using Microsoft.AspNetCore.Http;

namespace UserService.AzureFunction
{
    public class PostCreateUser
    {
        private readonly IMediator _mediator;

        public PostCreateUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostCreateUser")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PostCreateUserResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(PostCreateUserRequest), "product request")] PostCreateUserRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PostCreateUser");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PostCreateUserResponse response = await _mediator.Send(req);
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
