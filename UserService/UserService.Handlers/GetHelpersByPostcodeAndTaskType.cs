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

            var users = await _helperService.GetHelpersWithinRadius(request.Postcode, cancellationToken);       

            GetHelpersByPostcodeAndTaskTypeResponse response = new GetHelpersByPostcodeAndTaskTypeResponse
            {
                Users = users.Select(x => new HelperContactInformation
                {
                    UserID = x.ID,                  
                    IsVerified = x.IsVerified.HasValue && x.IsVerified.Value,
                    IsStreetChampionOfPostcode = x.ChampionPostcodes.Contains(request.Postcode),
                    SupportedActivites = x.SupportActivities

                }).Where(x => x.IsStreetChampionOfPostcode || x.SupportedActivites.Any(sa => request.RequestedTasks.SupportActivities.Any(ra => sa == ra))) .ToList()             
            };

            return response;
        }
    }
}
