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

                //  GetChampionsByPostcodeResponse response = await _mediator.Send(req);

                GetChampionsByPostcodeResponse response = new GetChampionsByPostcodeResponse()
                {
                    Users = new List<User>()
                    {
                        new User()
                        {
                            DateCreated = DateTime.Now.AddDays(-7),
                            HMSContactConsent = true,
                            ID = 3,
                            MobileSharingConsent = true,
                            SupportRadiusMiles= 3,
                            IsVolunteer = true,
                            IsVerified = true,
                            EmailSharingConsent = true,
                            SupportVolunteersByPhone = true,
                            StreetChampionRoleUnderstood = true,
                            OtherPhoneSharingConsent = true,
                            ChampionPostcodes = new List<string>()
                            {
                                req.PostCode
                            },
                            SupportActivities = new List<SupportActivities>()
                            {
                                SupportActivities.CheckingIn,
                                SupportActivities.CollectingPrescriptions,
                                SupportActivities.MealPreparation,
                                SupportActivities.Errands,
                                SupportActivities.MealPreparation,
                                SupportActivities.Shopping
                            },
                            UserPersonalDetails = new UserPersonalDetails()
                            {
                                Address = new Address()
                                {
                                    AddressLine1 = "11 Derby Road",
                                    AddressLine2 = "Mapperley",
                                    AddressLine3 = null,
                                    Locality = "Nottingham",
                                    Postcode = req.PostCode
                                },
                                DisplayName = "Pete",
                                FirstName = "Peter",
                                LastName = "Snow",
                                EmailAddress = "p.snow@gmail.com",
                                MobilePhone = "07954 452739",
                                UnderlyingMedicalCondition = false,
                                OtherPhone = null,
                                DateOfBirth = new DateTime(1970,08,12),
                            },
                            PostalCode = req.PostCode
                        }
                    }
                };
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
