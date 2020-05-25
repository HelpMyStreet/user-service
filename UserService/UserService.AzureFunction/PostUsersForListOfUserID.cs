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

namespace UserService.AzureFunction
{
    public class PostUsersForListOfUserID
    {
        private readonly IMediator _mediator;

        public PostUsersForListOfUserID(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostUsersForListOfUserID")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PostUsersForListOfUserIDResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(PostUsersForListOfUserIDRequest), "product request")] PostUsersForListOfUserIDRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PostUsersForListOfUserID");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PostUsersForListOfUserIDResponse response = await _mediator.Send(req);
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
