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
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.ReportService.Response;
using Microsoft.AspNetCore.Http;

namespace UserService.AzureFunction
{
    public class GetReport
    {
        private readonly IMediator _mediator;

        public GetReport(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Transaction(Web = true)]
        [FunctionName("GetReport")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetReportResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetReportRequest), "report request")] GetReportRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetReport");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetReportResponse response = await _mediator.Send(req);

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
