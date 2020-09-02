using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

namespace UserService.AzureFunction
{
    public class GetHelpersByPostcode
    {
        private readonly IMediator _mediator;

        public GetHelpersByPostcode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetHelpersByPostcode")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetHelpersByPostcodeRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetHelpersByPostcode");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetHelpersByPostcodeResponse response = await _mediator.Send(req);

                return new OkObjectResult(ResponseWrapper<GetHelpersByPostcodeResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetHelpersByPostcodeResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
