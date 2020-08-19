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
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

namespace UserService.AzureFunction
{
    public class GetChampionPostcodesCoveredCount
    {
        private readonly IMediator _mediator;

        public GetChampionPostcodesCoveredCount(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetChampionPostcodesCoveredCount")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetChampionPostcodesCoveredCountResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetChampionPostcodesCoveredCountRequest), "product request")] GetChampionPostcodesCoveredCountRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetChampionPostcodesCoveredCount");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetChampionPostcodesCoveredCountResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<GetChampionPostcodesCoveredCountResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetChampionPostcodesCoveredCountResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
