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
    public class GetHelpersByPostcodeHandler : IRequestHandler<GetHelpersByPostcodeRequest, GetHelpersByPostcodeResponse>
    {
        private readonly IHelperService _helperService;
        private readonly IRepository _repository;

        public GetHelpersByPostcodeHandler(IHelperService helperService, IRepository repository)
        {
            _helperService = helperService;
            _repository = repository;
        }

        public async Task<GetHelpersByPostcodeResponse> Handle(GetHelpersByPostcodeRequest request, CancellationToken cancellationToken)
        {
            request.Postcode = PostcodeFormatter.FormatPostcode(request.Postcode);

            var users = await _helperService.GetHelpersWithinRadius(request.Postcode, cancellationToken);
            
            GetHelpersByPostcodeResponse response = new GetHelpersByPostcodeResponse()
            {
                Users = users.ToList()
            };

            return response;
        }
    }
}
