using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;

namespace UserService.AzureFunction
{
    public class PostCreateChampionForPostCode
    {
        private readonly IMediator _mediator;

        public PostCreateChampionForPostCode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("PostCreateChampionForPostCode")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] PostCreateChampionForPostCodeRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                await _mediator.Send(req);
                return new NoContentResult();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
