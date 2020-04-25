using AzureFunctions.Extensions.Swashbuckle.Attribute;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;

namespace UserService.AzureFunction
{
    public class GetHelperCoordsByPostcodeAndRadius
    {
        private readonly IMediator _mediator;

        public GetHelperCoordsByPostcodeAndRadius(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetHelperCoordsByPostcodeAndRadius")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetHelpersByPostcodeResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetHelpersByPostcodeRequest), "product request")] GetHelpersByPostcodeRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetHelpersByPostcodeResponse response = await _mediator.Send(req);

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
