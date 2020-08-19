﻿using System.Threading.Tasks;
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
    public class GetDistinctChampionUserCount
    {
        private readonly IMediator _mediator;

        public GetDistinctChampionUserCount(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetDistinctChampionUserCount")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetDistinctChampionUserCountResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetDistinctChampionUserCountRequest), "product request")] GetDistinctChampionUserCountRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetDistinctChampionUserCount");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetDistinctChampionUserCountResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<GetDistinctChampionUserCountResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetDistinctChampionUserCountResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
