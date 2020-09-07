using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            PostCreateUserRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PostCreateUser");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PostCreateUserResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PostCreateUserResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PostCreateUserResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
