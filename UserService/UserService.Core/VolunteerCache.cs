using AddressService.Core.Contracts;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Contracts;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;

namespace UserService.Core
{
    public class VolunteerCache : IVolunteerCache
    {
        private static readonly Polly.Caching.Memory.MemoryCacheProvider _memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));

        private readonly IRepository _repository;
        private readonly IAddressService _addressService;
        private readonly IDateTime _mockableDateTime;
        private readonly IOptionsSnapshot<ApplicationConfig> _applicationConfig;

        public VolunteerCache(IRepository repository, IAddressService addressService, IDateTime mockableDateTime, IOptionsSnapshot<ApplicationConfig> applicationConfig)
        {
            _repository = repository;
            _addressService = addressService;
            _mockableDateTime = mockableDateTime;
            _applicationConfig = applicationConfig;
        }

        public async Task<IEnumerable<CachedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, IsVerifiedType isVerifiedType, CancellationToken cancellationToken)
        {
            CachePolicy cachePolicy = Policy.Cache(_memoryCacheProvider, GetLengthOfTimeUntilNextHour());

            Context context = new Context($"{nameof(VolunteerCache)}_{nameof(VolunteerCache.GetCachedVolunteersAsync)}");

            IEnumerable<CachedVolunteerDto> result = await cachePolicy.Execute(async _ => await GetAllVolunteersAsync(cancellationToken), context);
            
            result = result.Where(x => x.VolunteerType.HasFlag(volunteerType) && x.IsVerifiedType.HasFlag(isVerifiedType));

            return result;
        }

        private TimeSpan GetLengthOfTimeUntilNextHour()
        {
            DateTime timeNow = _mockableDateTime.UtcNow;
            DateTime nowPlusOneMinute = timeNow.AddHours(1);
            DateTime theNextMinuteWithoutSeconds = new DateTime(nowPlusOneMinute.Year, nowPlusOneMinute.Month, nowPlusOneMinute.Day, nowPlusOneMinute.Hour, 0, 0);
            TimeSpan timeSpanUntilNextMinute = theNextMinuteWithoutSeconds - timeNow;
            return timeSpanUntilNextMinute;
        }

        private async Task<IEnumerable<CachedVolunteerDto>> GetAllVolunteersAsync(CancellationToken cancellationToken)
        {
            int batchSize = _applicationConfig.Value.GetVolunteersForCacheBatchSize;

            int totalVolunteerCount = _repository.GetDistinctVolunteerUserCount();

            if (totalVolunteerCount == 0)
            {
                return new List<CachedVolunteerDto>();
            }

            int minUserId = await _repository.GetMinUserIdAsync();
            int maxUserId = await _repository.GetMaxUserIdAsync();

            // this is a rough calculation because not all IDs could be used, but is accurate enough (can't use totalVolunteerCount above because the User ID might not start at 1)
            int totalVolunteersCountForBatching  = maxUserId - minUserId + 1;
            
            int numberOfBatches = totalVolunteersCountForBatching  / batchSize;
            if (totalVolunteersCountForBatching  % batchSize > 0)
            {
                numberOfBatches++;
            }

            // get users from DB and call Address Service for coordinates in concurrent batches for speed

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

            List<VolunteerForCacheDto> volunteerForCacheDtos = new List<VolunteerForCacheDto>(totalVolunteersCountForBatching );
            List<Task<GetPostcodeCoordinatesResponse>> getPostcodeCoordinatesTasks = new List<Task<GetPostcodeCoordinatesResponse>>();
            while (volunteersInBatchTasks.Count > 0)
            {
                Task<IEnumerable<VolunteerForCacheDto>> finishedTask = await Task.WhenAny(volunteersInBatchTasks);
                volunteersInBatchTasks.Remove(finishedTask);

                IEnumerable<VolunteerForCacheDto> volunteersBatch = await finishedTask;

                volunteerForCacheDtos.AddRange(volunteersBatch);

                GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest();

                getPostcodeCoordinatesRequest.Postcodes = volunteersBatch.Select(x=>x.Postcode);

                Task<GetPostcodeCoordinatesResponse> getPostcodeCoordinatesResponseTask = _addressService.GetPostcodeCoordinatesAsync(getPostcodeCoordinatesRequest, cancellationToken);
                getPostcodeCoordinatesTasks.Add(getPostcodeCoordinatesResponseTask);
            }

            await Task.WhenAll(getPostcodeCoordinatesTasks);
            List<PostcodeCoordinate> postcodeCoordinates = new List<PostcodeCoordinate>(totalVolunteersCountForBatching );

            foreach (Task<GetPostcodeCoordinatesResponse> getPostcodeCoordinatesTask in getPostcodeCoordinatesTasks)
            {
                GetPostcodeCoordinatesResponse getPostcodeCoordinatesResponse = await getPostcodeCoordinatesTask;
                postcodeCoordinates.AddRange(getPostcodeCoordinatesResponse.PostcodeCoordinates);
            }

            List<CachedVolunteerDto> cachedVolunteerDtos =
                (from volunteerForCacheDto in volunteerForCacheDtos
                join postcodeCoordinate in postcodeCoordinates 
                    on volunteerForCacheDto.Postcode equals postcodeCoordinate.Postcode
                select new CachedVolunteerDto
                {
                    UserId = volunteerForCacheDto.UserId,
                    Postcode = volunteerForCacheDto.Postcode,
                    IsVerifiedType = volunteerForCacheDto.IsVerifiedType,
                    SupportRadiusMiles = volunteerForCacheDto.SupportRadiusMiles,
                    VolunteerType = volunteerForCacheDto.VolunteerType,
                    Latitude = postcodeCoordinate.Latitude,
                    Longitude = postcodeCoordinate.Longitude
                }).ToList();

            return cachedVolunteerDtos;
        }
    }
}
