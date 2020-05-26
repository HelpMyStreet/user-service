using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;

namespace UserService.Core
{
    public class VolunteersForCacheGetter : IVolunteersForCacheGetter
    {
        private readonly IRepository _repository;        
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public VolunteersForCacheGetter(IRepository repository, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _repository = repository;
            _applicationConfig = applicationConfig;
        }

        public async Task<IEnumerable<CachedVolunteerDto>> GetAllVolunteersAsync(CancellationToken cancellationToken)
        {
            int batchSize = _applicationConfig.Value.GetVolunteersForCacheBatchSize;

            int totalVolunteerCount = _repository.GetAllDistinctVolunteerUserCount();

            if (totalVolunteerCount == 0)
            {
                return new List<CachedVolunteerDto>();
            }

            int minUserId = await _repository.GetMinUserIdAsync();
            int maxUserId = await _repository.GetMaxUserIdAsync();

            // this is a rough calculation because not all IDs could be used, but is accurate enough (can't use totalVolunteerCount above because the User ID might not start at 1)
            int totalVolunteersCountForBatching = maxUserId - minUserId + 1;

            int numberOfBatches = totalVolunteersCountForBatching / batchSize;
            if (totalVolunteersCountForBatching % batchSize > 0)
            {
                numberOfBatches++;
            }
            
            List<Task<IEnumerable<VolunteerForCacheDto>>> volunteersInBatchTasks = new List<Task<IEnumerable<VolunteerForCacheDto>>>();

            int from = minUserId;
            int to = from + batchSize - 1;
            for (int i = 0; i < numberOfBatches; i++)
            {
                Task<IEnumerable<VolunteerForCacheDto>> volunteersInBatchTask = _repository.GetVolunteersForCacheAsync(from, to);
                volunteersInBatchTasks.Add(volunteersInBatchTask);

                from += batchSize;
                to += batchSize;
            }

            List<VolunteerForCacheDto> volunteerForCacheDtos = new List<VolunteerForCacheDto>(totalVolunteersCountForBatching);            
            while (volunteersInBatchTasks.Count > 0)
            {
                Task<IEnumerable<VolunteerForCacheDto>> finishedTask = await Task.WhenAny(volunteersInBatchTasks);
                volunteersInBatchTasks.Remove(finishedTask);

                IEnumerable<VolunteerForCacheDto> volunteersBatch = await finishedTask;

                volunteerForCacheDtos.AddRange(volunteersBatch);            
            }
            
            List<CachedVolunteerDto> cachedVolunteerDtos =
                (from volunteerForCacheDto in volunteerForCacheDtos
                 select new CachedVolunteerDto
                 {
                     UserId = volunteerForCacheDto.UserId,
                     Postcode = volunteerForCacheDto.Postcode,
                     IsVerifiedType = volunteerForCacheDto.IsVerifiedType,
                     SupportRadiusMiles = volunteerForCacheDto.SupportRadiusMiles,
                     VolunteerType = volunteerForCacheDto.VolunteerType,
                     Latitude = volunteerForCacheDto.Latitude,
                     Longitude = volunteerForCacheDto.Longitude
                 }).ToList();

            return cachedVolunteerDtos;
        }
    }
}
