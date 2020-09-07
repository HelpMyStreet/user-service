using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using NewRelic.Api.Agent;
using HelpMyStreet.Contracts.ReportService.Response;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Contracts.UserService.Response;

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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            GetReportRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetReport");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetReportResponse response = await _mediator.Send(req);

                return new OkObjectResult(ResponseWrapper<GetReportResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetReportResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
