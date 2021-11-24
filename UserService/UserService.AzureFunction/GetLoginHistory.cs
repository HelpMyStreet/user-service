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
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using UserService.Core.Contracts;

namespace UserService.AzureFunction
{
    public class GetLoginHistory
    {
        private readonly IMediator _mediator;

        public GetLoginHistory(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetLoginHistory")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetLoginHistoryResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetLoginHistoryRequest), "Get Login History")] GetLoginHistoryRequest req,
            ILogger log)
        {
            try
            {
                GetLoginHistoryResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<GetLoginHistoryResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetLoginHistoryResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
