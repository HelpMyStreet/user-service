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
    public class GetHelpersByPostcodeAndTaskTypeHandler : IRequestHandler<GetHelpersByPostcodeAndTaskTypeRequest, GetHelpersByPostcodeAndTaskTypeResponse>
    {
        private readonly IHelperService _helperService;
        private readonly IRepository _repository;

        public GetHelpersByPostcodeAndTaskTypeHandler(IHelperService helperService, IRepository repository)
        {
            _helperService = helperService;
            _repository = repository;
        } 

        public async Task<GetHelpersByPostcodeAndTaskTypeResponse> Handle(GetHelpersByPostcodeAndTaskTypeRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            var users = await _helperService.GetHelpersWithinRadius(request.Postcode, cancellationToken);       

            GetHelpersByPostcodeAndTaskTypeResponse response = new GetHelpersByPostcodeAndTaskTypeResponse
            {
                Users = users.Select(x => new HelperContactInformation
                {
                    Email = x.User.UserPersonalDetails.EmailAddress,
                    DisplayName = x.User.UserPersonalDetails.DisplayName,                  
                    IsVerified = x.User.IsVerified.HasValue && x.User.IsVerified.Value,
                    IsStreetChampionOfPostcode = x.User.ChampionPostcodes.Contains(request.Postcode),
                    SupportedActivites = x.User.SupportActivities,
                    DistanceFromPostcode = x.Distance,

                }).Where(x => x.IsStreetChampionOfPostcode || x.SupportedActivites.Any(sa => request.RequestedTasks.SupportActivities.Any(ra => sa == ra))) .ToList()             
            };

            return response;
        }
    }
}
