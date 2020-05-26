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
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 5d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 2,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 10d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 3,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 15d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 4,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 20d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 5,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 25d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 6,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 30d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 32,
                IsVerified = false,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 35d,
            });
            volunteers.Add(new VolunteerSummary()
            {
                UserID = 85,
                IsVerified = true,
                IsStreetChampionForGivenPostCode = false,
                DistanceInMiles = 40d
            });

            GetVolunteersByPostcodeAndActivityResponse response = new GetVolunteersByPostcodeAndActivityResponse()
            {
                Volunteers = volunteers
            };

            return response;
        }
    }
}
