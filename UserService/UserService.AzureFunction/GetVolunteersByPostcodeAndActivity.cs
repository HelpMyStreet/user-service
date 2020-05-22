using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;

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
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetVolunteersByPostcodeAndActivityResponse");
                log.LogInformation("C# HTTP trigger function processed a request.");
                GetVolunteersByPostcodeAndActivityResponse response = await _mediator.Send(req);
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
