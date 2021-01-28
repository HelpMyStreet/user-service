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

            var helpers = await _helperService.GetHelpersWithinRadius(request.Postcode, cancellationToken);
            
            GetHelpersByPostcodeResponse response = new GetHelpersByPostcodeResponse()
            {
                Users = helpers.Select(x => x.User).ToList()
            };

            return response;
        }
    }
}
