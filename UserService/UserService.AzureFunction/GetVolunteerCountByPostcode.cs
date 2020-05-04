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
using NewRelic.Api.Agent;

namespace UserService.AzureFunction
{
    public class GetVolunteerCountByPostcode
    {
        private readonly IMediator _mediator;

        public GetVolunteerCountByPostcode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetVolunteerCountByPostcode")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetVolunteerCountByPostcodeResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetVolunteerCountByPostcodeRequest), "product request")] GetVolunteerCountByPostcodeRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetVolunteerCountByPostcode");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetVolunteerCountByPostcodeResponse response = await _mediator.Send(req);
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
