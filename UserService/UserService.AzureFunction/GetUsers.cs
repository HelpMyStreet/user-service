using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using Microsoft.AspNetCore.Http;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

namespace UserService.AzureFunction
{
    public class GetUsers
    {
        private readonly IMediator _mediator;

        public GetUsers(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetUsers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetUsersRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetUsersResponse");
                log.LogInformation("C# HTTP trigger function processed a request.");
                GetUsersResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<GetUsersResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetUsersResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
