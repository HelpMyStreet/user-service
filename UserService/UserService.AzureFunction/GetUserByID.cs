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
using NewRelic.Api.Agent;

namespace UserService.AzureFunction
{
    public class GetUserByID
    {
        private readonly IMediator _mediator;

        public GetUserByID(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetUserByID")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUserByIDResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetUserByIDRequest), "product request")] GetUserByIDRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetUserByID");
                log.LogInformation("C# HTTP trigger function processed a request.");

                var eventAttributes = new Dictionary<string, object>() { { "userID", req.ID.ToString() } };
                NewRelic.Api.Agent.NewRelic.RecordCustomEvent("GetChampionCountByPostcode request", eventAttributes);

                GetUserByIDResponse response = await _mediator.Send(req);
                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
