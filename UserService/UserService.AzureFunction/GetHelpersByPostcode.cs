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
    public class GetHelpersByPostcode
    {
        private readonly IMediator _mediator;

        public GetHelpersByPostcode(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("GetHelpersByPostcode")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetHelpersByPostcodeResponse))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            [RequestBodyType(typeof(GetHelpersByPostcodeRequest), "product request")] GetHelpersByPostcodeRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                // GetVolunteersByPostcodeResponse response = await _mediator.Send(req);

                GetHelpersByPostcodeResponse response = new GetHelpersByPostcodeResponse()
                {
                    Users = new List<User>()
                    {
                        new User()
                        {
                            DateCreated = DateTime.Now.AddDays(-7),
                            HMSContactConsent = true,
                            ID = 1,
                            MobileSharingConsent = true,
                            SupportRadiusMiles= 3,
                            IsVolunteer = true,
                            IsVerified = true,

                            EmailSharingConsent = true,
                            SupportVolunteersByPhone = true,
                            StreetChampionRoleUnderstood = null,
                            OtherPhoneSharingConsent = true,

                            SupportPostcodes = new List<string>()
                            {
                                req.PostCode, "NG1 5FS"
                            },

                            SupportActivities = new List<SupportActivities>()
                            {
                                SupportActivities.CheckingIn,
                                SupportActivities.CollectingPrescriptions,
                                SupportActivities.MealPreparation
                            },
                            UserPersonalDetails = new UserPersonalDetails()
                            {
                                Address = new Address()
                                {
                                    AddressLine1 = "Flat 1 White House",
                                    AddressLine2 = "Bakery Lane",
                                    AddressLine3 = "Loscoe",
                                    Locality = "Malad East",
                                    Postcode = req.PostCode
                                },
                                DisplayName = "Gloria",
                                FirstName = "Gloria",
                                LastName = "Kennedy",
                                EmailAddress = "g.kennedy@gmail.com",
                                MobilePhone = "07854 759954",
                                UnderlyingMedicalCondition = false,
                                OtherPhone = "0115 9605745",
                                DateOfBirth = new DateTime(1975,03,21),
                            },

                            PostalCode = req.PostCode
                        },
                         new User()
                        {
                            DateCreated = DateTime.Now.AddDays(-6),
                            HMSContactConsent = true,
                            ID = 2,
                            MobileSharingConsent = true,
                            SupportRadiusMiles= 1,
                            IsVolunteer = true,
                            IsVerified = true,

                            EmailSharingConsent = true,
                            SupportVolunteersByPhone = true,
                            StreetChampionRoleUnderstood = null,
                            OtherPhoneSharingConsent = true,

                            SupportPostcodes = new List<string>()
                            {
                                req.PostCode, "NG1 5BA",  "NG1 6DQ"
                            },
                            SupportActivities = new List<SupportActivities>()
                            {
                                SupportActivities.Errands,
                                SupportActivities.HomeworkSupport,
                                SupportActivities.DogWalking
                            },
                            UserPersonalDetails = new UserPersonalDetails()
                            {
                                Address = new Address()
                                {
                                    AddressLine1 = "Flat 7 White House",
                                    AddressLine2 = "Bakery Lane",
                                    AddressLine3 = "Loscoe",
                                    Locality = "Malad East",
                                    Postcode = req.PostCode
                                },
                                DisplayName = "James",
                                FirstName = "James",
                                LastName = "Khan",
                                EmailAddress = "j.khan@gmail.com",
                                MobilePhone = "07854 784991",
                                UnderlyingMedicalCondition = false,
                                OtherPhone = null,
                                DateOfBirth = new DateTime(1979,04,02),
                            },
                            PostalCode = req.PostCode
                        },
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
                            SupportPostcodes = new List<string>(),
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
