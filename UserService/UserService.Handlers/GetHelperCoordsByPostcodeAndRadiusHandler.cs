using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Contracts;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;

namespace UserService.Handlers
{
    public class GetHelperCoordsByPostcodeAndRadiusHandler : IRequestHandler<GetHelperCoordsByPostcodeAndRadiusRequest, GetHelperCoordsByPostcodeAndRadiusResponse>
    {
        private readonly IRepository _repository;
        private readonly IAddressService _addressService;
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public GetHelperCoordsByPostcodeAndRadiusHandler(IRepository repository, IAddressService addressService, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _repository = repository;
            _addressService = addressService;
            _applicationConfig = applicationConfig;
        }

        public async Task<GetHelperCoordsByPostcodeAndRadiusResponse> Handle(GetHelperCoordsByPostcodeAndRadiusRequest request, CancellationToken cancellationToken)
        {
            
            return new GetHelperCoordsByPostcodeAndRadiusResponse();
        }
    }
}
