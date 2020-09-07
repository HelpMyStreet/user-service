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

namespace UserService.Core.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public CommunicationService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<bool> RequestCommunicationAsync(RequestCommunicationRequest requestCommunicationRequest, CancellationToken cancellationToken)
        {
            string path = $"api/RequestCommunication";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestCommunicationRequest), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _httpClientWrapper.PostAsync(HttpClientConfigName.CommunicationService, path, jsonContent, cancellationToken).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var emailSentResponse = JsonConvert.DeserializeObject<ResponseWrapper<RequestCommunicationResponse, CommunicationServiceErrorCode>>(jsonResponse);
                if (emailSentResponse.HasContent && emailSentResponse.IsSuccessful)
                {
                    return emailSentResponse.Content.Success;
                }
                return false;
            }
        }
    }
}
