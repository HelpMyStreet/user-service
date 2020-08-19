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
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

namespace UserService.AzureFunction
{
    public class DeleteUser
    {
        private readonly IMediator _mediator;

        public DeleteUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("DeleteUser")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteUserResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)]
            [RequestBodyType(typeof(DeleteUserRequest), "product request")] DeleteUserRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "DeleteUser");
                log.LogInformation("C# HTTP trigger function processed a request.");

                DeleteUserResponse response = await _mediator.Send(req);

                return new OkObjectResult(ResponseWrapper<DeleteUserResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<DeleteUserResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
