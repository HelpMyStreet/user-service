using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using System.Collections.Generic;

namespace UserService.Handlers
{
    public class GetVolunteersByPostcodeAndActivityHandler : IRequestHandler<GetVolunteersByPostcodeAndActivityRequest, GetVolunteersByPostcodeAndActivityResponse>
    {
        public async Task<GetVolunteersByPostcodeAndActivityResponse> Handle(GetVolunteersByPostcodeAndActivityRequest request, CancellationToken cancellationToken)
        {
            List<VolunteerSummary> volunteers = new List<VolunteerSummary>();

            volunteers.Add(new VolunteerSummary()
            {
                UserID = 1,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 2,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 3,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 4,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 5,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 6,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 32,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 85,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false
            });

            GetVolunteersByPostcodeAndActivityResponse response = new GetVolunteersByPostcodeAndActivityResponse()
            {
                Volunteers = volunteers
            };

            return response;
        }
    }
}
