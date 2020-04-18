using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;

namespace UserService.Core.Interfaces.Utils
{
    public interface IHttpClientWrapper
    { Task<HttpResponseMessage> GetAsync(HttpClientConfigName httpClientConfigName, string absolutePath, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(HttpClientConfigName httpClientConfigName, string absolutePath, HttpContent content, CancellationToken cancellationToken);
    }
}