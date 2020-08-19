using AzureFunctions.Extensions.Swashbuckle.Attribute;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NewRelic.Api.Agent;
using System;
using System.Net;
using System.Threading.Tasks;

namespace UserService.AzureFunction
{
    public class GetVolunteersByPostcodeAndActivity
    {
        private readonly IMediator _mediator;

        public GetVolunteersByPostcodeAndActivity(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetVolunteersByPostcodeAndActivity")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetVolunteersByPostcodeAndActivityResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetVolunteersByPostcodeAndActivityRequest), "product request")] GetVolunteersByPostcodeAndActivityRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetHelpersContactInformationByPostcode");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetVolunteersByPostcodeAndActivityResponse response = await _mediator.Send(req);

                return new OkObjectResult(ResponseWrapper<GetVolunteersByPostcodeAndActivityResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);
                return new ObjectResult(ResponseWrapper<GetVolunteersByPostcodeAndActivityResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
