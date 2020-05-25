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

namespace UserService.Handlers
{
    public class GetHelpersByPostcodeAndTaskType : IRequestHandler<GetHelpersByPostcodeAndTaskTypeRequest, GetHelpersByPostcodeAndTaskTypeResponse>
    {
        private readonly IHelperService _helperService;
        private readonly IRepository _repository;

        public GetHelpersByPostcodeAndTaskType(IHelperService helperService, IRepository repository)
        {
            _helperService = helperService;
            _repository = repository;
        } 

        public async Task<GetHelpersByPostcodeAndTaskTypeResponse> Handle(GetHelpersByPostcodeAndTaskTypeRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            var idsOfHelpersWithinRadius = await _helperService.GetHelpersWithinRadius(request.Postcode, cancellationToken);
            IEnumerable<User> users = await _repository.GetVolunteersByIdsAndSupportActivitesAsync(idsOfHelpersWithinRadius, request.RequestedTasks.SupportActivities);

            GetHelpersByPostcodeAndTaskTypeResponse response = new GetHelpersByPostcodeAndTaskTypeResponse
            {
                Users = users.Select(x => new HelperContactInformation
                {
                    DisplayName = x.UserPersonalDetails.DisplayName,
                    Email = x.UserPersonalDetails.EmailAddress,
                    IsVerified = x.IsVerified.HasValue && x.IsVerified.Value,
                    IsStreetChampionOfPostcode = x.ChampionPostcodes.Contains(request.Postcode)
                }).ToList()                
            };

            return response;
        }
    }
}
