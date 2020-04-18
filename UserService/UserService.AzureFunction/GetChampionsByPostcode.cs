using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using UserService.Core.Domains.Entities;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace UserService.AzureFunction
{
    public class GetChampionsByPostcode
    {
        private readonly IMediator _mediator;

        public GetChampionsByPostcode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetChampionsByPostcode")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetChampionsByPostcodeResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetChampionsByPostcodeRequest), "product request")] GetChampionsByPostcodeRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetChampionsByPostcodeResponse response = await _mediator.Send(req);

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
