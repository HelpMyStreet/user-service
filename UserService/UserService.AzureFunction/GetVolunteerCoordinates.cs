using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace UserService.AzureFunction
{
    public class GetVolunteerCoordinates
    {
        private readonly IMediator _mediator;

        public GetVolunteerCoordinates(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetVolunteerCoordinates")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetVolunteerCoordinatesResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetVolunteerCoordinatesRequest), "Get Volunteer Coordinates")] GetVolunteerCoordinatesRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "GetVolunteerCoordinates");
                log.LogInformation("C# HTTP trigger function processed a request.");

                GetVolunteerCoordinatesResponse response = await _mediator.Send(req);

                return new OkObjectResult(ResponseWrapper<GetVolunteerCoordinatesResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<GetVolunteerCoordinatesResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
