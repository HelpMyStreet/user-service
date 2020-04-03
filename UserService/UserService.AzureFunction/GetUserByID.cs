using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using UserService.Core.Domains.Entities;
using HelpMyStreet.Utils.Models;
using Newtonsoft.Json;

namespace UserService.AzureFunction
{
    public class GetUserByID
    {
        private readonly IMediator _mediator;

        public GetUserByID(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetUserByID")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] GetUserByIDRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                User user = new User()
                {
                    UserPersonalDetails = new UserPersonalDetails()
                    {
                        FirstName = "John",
                        LastName = "Smith",
                        DisplayName = "John Smith",
                        EmailAddress = "john@smith.com",
                        MobilePhone = "07",
                        OtherPhone = "01332",
                        DateOfBirth = new DateTime(1900, 1, 1),
                        Address = new Address()
                        {
                            AddressLine1 = "1 Test Street",
                            AddressLine2 = "Test Town",
                            Postcode = "BA133BN"
                        },
                        UnderlyingMedicalCondition = false
                    },
                    PostalCode = "BA133BN"
                };

                string jsonData = JsonConvert.SerializeObject(user);



                GetUserByIDResponse response = await _mediator.Send(req);
                return new OkObjectResult(response);
            }
            catch (Exception exc)
            {
                log.LogInformation(exc.ToString());
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
