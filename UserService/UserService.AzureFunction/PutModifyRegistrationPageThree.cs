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
    public class PutModifyRegistrationPageThree
    {
        private readonly IMediator _mediator;

        public PutModifyRegistrationPageThree(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PutModifyRegistrationPageThree")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]
            PutModifyRegistrationPageThreeRequest req,
        ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PutModifyRegistrationPageThree");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PutModifyRegistrationPageThreeResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PutModifyRegistrationPageThreeResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PutModifyRegistrationPageThreeResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
