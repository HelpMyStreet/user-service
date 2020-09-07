using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using NewRelic.Api.Agent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

namespace UserService.AzureFunction
{
    public class GetChampionCountByPostcode
    {
        private readonly IMediator _mediator;

        public GetChampionCountByPostcode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction (Web = true)]
        [FunctionName("GetChampionCountByPostcode")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetChampionCountByPostcodeRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetChampionCountByPostcode");
                
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetChampionCountByPostcodeResponse response = await _mediator.Send(req);

                var eventAttributes = new Dictionary<string, object>() { { "result", "Success!" }, {"PostCode",req.PostCode },{ "Count", response.Count.ToString() } };
                NewRelic.Api.Agent.NewRelic.RecordCustomEvent("GetChampionCountByPostcode response", eventAttributes);

                return new OkObjectResult(ResponseWrapper<GetChampionCountByPostcodeResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetChampionCountByPostcodeResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
