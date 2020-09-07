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
    public class PutModifyUser
    {
        private readonly IMediator _mediator;

        public PutModifyUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PutModifyUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]
            PutModifyUserRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PutModifyUser");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PutModifyUserResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PutModifyUserResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PutModifyUserResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
