using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;
using System.Net;
using System.Threading;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using Microsoft.AspNetCore.Http;
using NewRelic.Api.Agent;
using UserService.Core.PreCalculation;

namespace UserService.AzureFunction
{
    public class PrecalulateData
    {
        private readonly IMediator _mediator;

        public PrecalulateData(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction (Web = true)]
        [FunctionName(nameof(PrecalulateData))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetChampionCountByPostcodeResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(PrecalulateDataRequest), "product request")]PrecalulateDataRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                PrecalulateDataResponse response = await _mediator.Send(req);

                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc);
                return new ObjectResult(exc) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
