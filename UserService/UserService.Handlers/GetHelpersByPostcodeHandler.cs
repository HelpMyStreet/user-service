using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;

namespace UserService.Handlers
{
    public class GetHelpersByPostcodeHandler : IRequestHandler<GetHelpersByPostcodeRequest, GetHelpersByPostcodeResponse>
    {
        private readonly IRepository _repository;
        private readonly IAddressService _addressService;
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public GetHelpersByPostcodeHandler(IRepository repository, IAddressService addressService, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _repository = repository;
            _addressService = addressService;
            _applicationConfig = applicationConfig;
        }

        public async Task<GetHelpersByPostcodeResponse> Handle(GetHelpersByPostcodeRequest request, CancellationToken cancellationToken)
        {
            int batchSize = _applicationConfig.Value.GetHelpersByPostcodeBatchSize;

            int minUserId = await _repository.GetMinUserIdAsync();
            int maxUserId = await _repository.GetMaxUserIdAsync();

            int totalVolunteers = maxUserId - minUserId;

            int numberOfBatches = totalVolunteers / batchSize;
            if (totalVolunteers % batchSize > 0)
            {
                numberOfBatches++;
            }

            // get users from DB and call Address Service in concurrent batches for speed

            List<Task<IEnumerable<HelperPostcodeRadiusDto>>> volunteersInBatchTasks = new List<Task<IEnumerable<HelperPostcodeRadiusDto>>>();

            int from = minUserId;
            int to = from + batchSize - 1;
            for (int i = 0; i < numberOfBatches; i++)
            {
                Task<IEnumerable<HelperPostcodeRadiusDto>> volunteersInBatchTask = _repository.GetVolunteersPostcodeRadiiAsync(from, to);
                volunteersInBatchTasks.Add(volunteersInBatchTask);

                from += batchSize;
                to += batchSize;
            }

            List<Task<IsPostcodeWithinRadiiResponse>> isPostcodeWithinRadiiTasks = new List<Task<IsPostcodeWithinRadiiResponse>>();
            while (volunteersInBatchTasks.Count > 0)
            {
                Task<IEnumerable<HelperPostcodeRadiusDto>> finishedTask = await Task.WhenAny(volunteersInBatchTasks);
                volunteersInBatchTasks.Remove(finishedTask);

                IEnumerable<HelperPostcodeRadiusDto> volunteersBatch = await finishedTask;

                IsPostcodeWithinRadiiRequest isPostcodeWithinRadiiRequest = new IsPostcodeWithinRadiiRequest();

                isPostcodeWithinRadiiRequest.Postcode = PostcodeFormatter.FormatPostcode(request.PostCode);
                isPostcodeWithinRadiiRequest.PostcodeWithRadiuses = volunteersBatch.Select(x => new PostcodeWithRadius()
                {
                    Id = x.UserId,
                    Postcode = x.Postcode,
                    RadiusInMetres = DistanceConverter.MilesToMetres(x.SupportRadiusMiles)
                }).ToList();

                Task<IsPostcodeWithinRadiiResponse> isPostcodeWithinRadiiTask = _addressService.IsPostcodeWithinRadiiAsync(isPostcodeWithinRadiiRequest, cancellationToken);
                isPostcodeWithinRadiiTasks.Add(isPostcodeWithinRadiiTask);
            }

            List<int> userIdsWithinRadiusOfPostcode = new List<int>();
            while (isPostcodeWithinRadiiTasks.Count > 0)
            {
                Task<IsPostcodeWithinRadiiResponse> finishedTask = await Task.WhenAny(isPostcodeWithinRadiiTasks);
                isPostcodeWithinRadiiTasks.Remove(finishedTask);

                IsPostcodeWithinRadiiResponse isPostcodeWithinRadiiBatch = await finishedTask;
                userIdsWithinRadiusOfPostcode.AddRange(isPostcodeWithinRadiiBatch.IdsWithinRadius);
            }

            IEnumerable<User> users = await _repository.GetVolunteersBIds(userIdsWithinRadiusOfPostcode);

            GetHelpersByPostcodeResponse response = new GetHelpersByPostcodeResponse()
            {
                Users = users.ToList()
            };

            return response;
        }
    }
}
