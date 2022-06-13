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
using System;

namespace UserService.Core.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public CommunicationService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<bool> DeleteMarketingContactAsync(DeleteMarketingContactRequest request, CancellationToken cancellationToken)
        {
            string path = $"api/DeleteMarketingContact";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _httpClientWrapper.DeleteAsync(HttpClientConfigName.CommunicationService, path, jsonContent, cancellationToken).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var deleteMarketingContactResponse = JsonConvert.DeserializeObject<ResponseWrapper<bool, CommunicationServiceErrorCode>>(jsonResponse);
                if (deleteMarketingContactResponse.HasContent && deleteMarketingContactResponse.IsSuccessful)
                {
                    return deleteMarketingContactResponse.Content;
                }
                return false;
            }
        }

        public async Task<DateTime?> GetDateEmailLastSentAsync(GetDateEmailLastSentRequest request, CancellationToken cancellationToken)
        {
            string path = $"api/GetDateEmailLastSent";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.CommunicationService, path, jsonContent, cancellationToken).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var getDateEmailLastSentResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetDateEmailLastSentResponse, CommunicationServiceErrorCode>>(jsonResponse);
                if (getDateEmailLastSentResponse.HasContent && getDateEmailLastSentResponse.IsSuccessful)
                {
                    return getDateEmailLastSentResponse.Content.DateEmailSent;
                }
                return null;
            }
        }

        public async Task<bool> PutNewMarketingContactAsync(PutNewMarketingContactRequest request, CancellationToken cancellationToken)
        {
            string path = $"api/PutNewMarketingContact";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _httpClientWrapper.PutAsync(HttpClientConfigName.CommunicationService, path, jsonContent, cancellationToken).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var putNewMarketingContactResponse = JsonConvert.DeserializeObject<ResponseWrapper<bool, CommunicationServiceErrorCode>>(jsonResponse);
                if (putNewMarketingContactResponse.HasContent && putNewMarketingContactResponse.IsSuccessful)
                {
                    return putNewMarketingContactResponse.Content;
                }
                return false;
            }
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
