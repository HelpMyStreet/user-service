using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;

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

            var users = await _helperService.GetHelpersWithinRadius(request.VolunteerFilter.Postcode, IsVerifiedType.All,  cancellationToken);
            
            GetVolunteersByPostcodeAndActivityResponse response = new GetVolunteersByPostcodeAndActivityResponse
            {
                Volunteers = users.Where(x => x.User.ChampionPostcodes.Contains(request.VolunteerFilter.Postcode) || x.User.SupportActivities.Any(sa => request.VolunteerFilter.Activities.Any(ra => sa == ra)))
                .Select(x => new VolunteerSummary
                {
                    UserID = x.User.ID,
                    IsVerified = x.User.IsVerified.HasValue && x.User.IsVerified.Value,
                    IsStreetChampionForGivenPostCode = x.User.ChampionPostcodes.Contains(request.VolunteerFilter.Postcode),
                    DistanceInMiles = x.Distance
                }).ToList()
            };

            return response;
        }
    }
}
