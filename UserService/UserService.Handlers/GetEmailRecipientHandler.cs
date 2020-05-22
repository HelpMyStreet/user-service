using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using System.Collections.Generic;

namespace UserService.Handlers
{
    public class GetEmailRecipientHandler : IRequestHandler<GetEmailRecipientRequest, GetEmailRecipientResponse>
    {
        public async Task<GetEmailRecipientResponse> Handle(GetEmailRecipientRequest request, CancellationToken cancellationToken)
        {
            List<EmailRecipient> emailRecipients = new List<EmailRecipient>();

            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 1,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 2,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 3,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 4,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 5,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 6,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 32,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            emailRecipients.Add(new EmailRecipient()
            {
                UserID = 85,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });

            GetEmailRecipientResponse response = new GetEmailRecipientResponse()
            {
                EmailRecipients = emailRecipients
            };

            return response;
        }
    }
}
