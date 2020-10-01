using HelpMyStreet.Contracts.CommunicationService.Request;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Services
{
    public interface ICommunicationService
    {
        Task<bool> RequestCommunicationAsync(RequestCommunicationRequest requestCommunicationRequest, CancellationToken cancellationToken);
        Task<bool> PutNewMarketingContactAsync(PutNewMarketingContactRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteMarketingContactAsync(DeleteMarketingContactRequest request, CancellationToken cancellationToken);
    }
}