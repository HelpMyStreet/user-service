using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.Shared;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace UserService.AzureFunction
{
    public class PostAddBiography
    {
        private readonly IMediator _mediator;

        public PostAddBiography(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostAddBiography")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PostAddBiographyResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            [RequestBodyType(typeof(PostAddBiographyRequest), "Post Add Biography")] PostAddBiographyRequest req,
            ILogger log)
        {
            try
            {
                NewRelic.Api.Agent.NewRelic.SetTransactionName("UserService", "PostAddBiography");
                log.LogInformation("C# HTTP trigger function processed a request.");

                PostAddBiographyResponse response = await _mediator.Send(req);
                return new OkObjectResult(ResponseWrapper<PostAddBiographyResponse, UserServiceErrorCode>.CreateSuccessfulResponse(response));
            }
            catch (Exception exc)
            {
                LogError.Log(log, exc, req);

                return new ObjectResult(ResponseWrapper<PostAddBiographyResponse, UserServiceErrorCode>.CreateUnsuccessfulResponse(UserServiceErrorCode.UnhandledError, "Internal Error")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
