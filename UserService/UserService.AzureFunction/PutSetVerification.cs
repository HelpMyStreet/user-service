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
    public class PutSetVerification
    {
        private readonly IMediator _mediator;

        public PutSetVerification(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PutSetVerification")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PutSetVerificationResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]
            [RequestBodyType(typeof(PutSetVerificationRequest), "Put Set Verification")]  PutSetVerificationRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PutSetVerification");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PutSetVerificationResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PutSetVerificationResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PutSetVerificationResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
