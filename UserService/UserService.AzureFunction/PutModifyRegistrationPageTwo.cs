﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using System.Net;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;

namespace UserService.AzureFunction
{
    public class PutModifyRegistrationPageTwo
    {
        private readonly IMediator _mediator;

        public PutModifyRegistrationPageTwo(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PutModifyRegistrationPageTwo")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PutModifyRegistrationPageTwoResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]
            [RequestBodyType(typeof(PutModifyRegistrationPageTwoRequest), "product request")]PutModifyRegistrationPageTwoRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PutModifyRegistrationPageTwo");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PutModifyRegistrationPageTwoResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PutModifyRegistrationPageTwoResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PutModifyRegistrationPageTwoResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
