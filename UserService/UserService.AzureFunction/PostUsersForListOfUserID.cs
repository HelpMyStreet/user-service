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
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace UserService.AzureFunction
{
    public class PostUsersForListOfUserID
    {
        private readonly IMediator _mediator;

        public PostUsersForListOfUserID(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostUsersForListOfUserID")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PostUsersForListOfUserIDResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(PostUsersForListOfUserIDRequest), "Post Users For List Of UserID")]  PostUsersForListOfUserIDRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PostUsersForListOfUserID");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PostUsersForListOfUserIDResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PostUsersForListOfUserIDResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PostUsersForListOfUserIDResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
