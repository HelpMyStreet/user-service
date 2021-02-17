using HelpMyStreet.Contracts.Shared;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.CommunicationService.Response;
using HelpMyStreet.Contracts.CommunicationService.Request;
using System.Text;
using Newtonsoft.Json;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Contracts.GroupService.Response;
using System;

namespace UserService.Core.Services
{
    public class GroupService : IGroupService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public GroupService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<List<SupportActivityDetail>> GetRegistrationFormSupportActivities(RegistrationFormVariant registrationFormVariant, CancellationToken cancellationToken)
        {
            GetRegistrationFormSupportActivitiesRequest request = new GetRegistrationFormSupportActivitiesRequest()
            {
                RegistrationFormVariantRequest = new RegistrationFormVariantRequest()
                {
                    RegistrationFormVariant = registrationFormVariant
                }
            };

            string path = $"api/GetRegistrationFormSupportActivities";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.GroupService, path, jsonContent, cancellationToken).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var emailSentResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetRegistrationFormSupportActivitiesResponse, CommunicationServiceErrorCode>>(jsonResponse);
                if (emailSentResponse.HasContent && emailSentResponse.IsSuccessful)
                {
                    return emailSentResponse.Content.SupportActivityDetails;
                }
                else
                {
                    throw new Exception($"Unable to retrieve support activities for registration form");
                }
            }
        }

        public async Task<List<SupportActivityConfiguration>> GetSupportActivitiesConfigurationAsync(CancellationToken cancellationToken)
        {
            GetSupportActivitiesConfigurationRequest request = new GetSupportActivitiesConfigurationRequest();
            string path = $"api/GetSupportActivitiesConfiguration";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.GroupService, path, jsonContent, cancellationToken).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var emailSentResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetSupportActivitiesConfigurationResponse, CommunicationServiceErrorCode>>(jsonResponse);
                if (emailSentResponse.HasContent && emailSentResponse.IsSuccessful)
                {
                    return emailSentResponse.Content.SupportActivityConfigurations;
                }
                else
                {
                    throw new Exception($"Unable to retrieve support activities");
                }
            }
        }
    }
}
