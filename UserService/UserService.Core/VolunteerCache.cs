using EnumsNET;
using Microsoft.Extensions.Internal;
using Polly;
using Polly.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Contracts;
using UserService.Core.Dto;

namespace UserService.Core
{
    public class VolunteerCache : IVolunteerCache
    {

        private readonly IPollyMemoryCacheProvider _pollyMemoryCacheProvider;
        private readonly IVolunteersForCacheGetter _volunteersForCacheGetter;
        private readonly ISystemClock _mockableDateTime;

        public VolunteerCache(IPollyMemoryCacheProvider pollyMemoryCacheProvider, IVolunteersForCacheGetter volunteersForCacheGetter, ISystemClock mockableDateTime)
        {
            _pollyMemoryCacheProvider = pollyMemoryCacheProvider;
            _volunteersForCacheGetter = volunteersForCacheGetter;
            _mockableDateTime = mockableDateTime;
        }

        /// <summary>
        /// Get volunteers from cache. Cache expires on the hour so all servers are kept in sync.
        /// </summary>
        public async Task<IEnumerable<CachedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, IsVerifiedType isVerifiedType, CancellationToken cancellationToken)
        {
            CachePolicy cachePolicy = Policy.Cache(_pollyMemoryCacheProvider.MemoryCacheProvider, GetLengthOfTimeUntilNextHour());

            Context context = new Context($"{nameof(VolunteerCache)}_{nameof(VolunteerCache.GetCachedVolunteersAsync)}");

            IEnumerable<CachedVolunteerDto> result = await cachePolicy.Execute(async _ => await _volunteersForCacheGetter.GetAllVolunteersAsync(cancellationToken), context);

            List<CachedVolunteerDto> matchingVolunteers = result.Where(x => x.VolunteerType.HasAnyFlags(volunteerType) && x.IsVerifiedType.HasAnyFlags(isVerifiedType)).ToList();

            return matchingVolunteers;
        }

        private TimeSpan GetLengthOfTimeUntilNextHour()
        {
            DateTimeOffset timeNow = _mockableDateTime.UtcNow;
            DateTimeOffset nowPlusOneMinute = timeNow.AddHours(1);
            DateTimeOffset theNextMinuteWithoutSeconds = new DateTime(nowPlusOneMinute.Year, nowPlusOneMinute.Month, nowPlusOneMinute.Day, nowPlusOneMinute.Hour, 0, 0);
            TimeSpan timeSpanUntilNextMinute = theNextMinuteWithoutSeconds - timeNow;
            return timeSpanUntilNextMinute;
        }
    }
}
