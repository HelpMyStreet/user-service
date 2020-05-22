using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;
using HelpMyStreet.Utils.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Response;

namespace UserService.AzureFunction
{
    public class GetUsers
    {
        private readonly IMediator _mediator;

        public GetUsers(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetUsers")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUsersResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetUsersRequest), "product request")] GetUsersRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetUsersResponse");
                log.LogInformation("C# HTTP trigger function processed a request.");
                GetUsersResponse response = await _mediator.Send(req);
                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(exc)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
