using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace UserService.Handlers
{
    public class GetVolunteersByPostcodeAndActivityHandler : IRequestHandler<GetVolunteersByPostcodeAndActivityRequest, GetVolunteersByPostcodeAndActivityResponse>
    {
        private readonly IHelperService _helperService;
        private readonly IRepository _repository;

        public GetVolunteersByPostcodeAndActivityHandler(IHelperService helperService, IRepository repository)
        {
            _helperService = helperService;
            _repository = repository;
        } 

        public async Task<GetVolunteersByPostcodeAndActivityResponse> Handle(GetVolunteersByPostcodeAndActivityRequest request, CancellationToken cancellationToken)
        {
            request.VolunteerFilter.Postcode = PostcodeFormatter.FormatPostcode(request.VolunteerFilter.Postcode);

            var users = await _helperService.GetHelpersWithinRadius(request.VolunteerFilter.Postcode, request.VolunteerFilter.OverrideVolunteerRadius,  cancellationToken);
            
            GetVolunteersByPostcodeAndActivityResponse response = new GetVolunteersByPostcodeAndActivityResponse
            {
                Volunteers = users.Where(x => x.User.SupportActivities.Any(sa => request.VolunteerFilter.Activities.Any(ra => sa == ra)))
                .Select(x => new VolunteerSummary
                {
                    UserID = x.User.ID,
                    DistanceInMiles = x.Distance
                }).ToList()
            };

            return response;
        }
    }
}
