using Microsoft.Extensions.Internal;
using Polly;
using Polly.Caching;
using System;
using System.Threading.Tasks;
using Polly.Contrib.DuplicateRequestCollapser;
using UserService.Core.Cache;

namespace UserService.Core
{
    public class CoordinatedResetCache : ICoordinatedResetCache
    {
        private readonly IPollyMemoryCacheProvider _pollyMemoryCacheProvider;
        private readonly IVolunteersForCacheGetter _volunteersForCacheGetter;
        private readonly ISystemClock _mockableDateTime;

        private static readonly IAsyncRequestCollapserPolicy _collapserPolicy = AsyncRequestCollapserPolicy.Create();

        public CoordinatedResetCache(IPollyMemoryCacheProvider pollyMemoryCacheProvider, ISystemClock mockableDateTime)
        {
            _pollyMemoryCacheProvider = pollyMemoryCacheProvider;
            _mockableDateTime = mockableDateTime;
        }

        public async Task<T> GetCachedDataAsync<T>(Func<Task<T>> dataGetter, string key, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour)
        {
            TimeSpan timeToReset;

            switch (resetCacheTime)
            {
                case CoordinatedResetCacheTime.OnHour:
                    timeToReset = GetLengthOfTimeUntilNextHour();
                    break;
                case CoordinatedResetCacheTime.OnMinute:
                    timeToReset = GetLengthOfTimeUntilNextMinute();
                    break;
            }

            AsyncCachePolicy cachePolicy = Policy.CacheAsync(_pollyMemoryCacheProvider.MemoryCacheProvider, timeToReset);

            Context context = new Context($"{nameof(CoordinatedResetCache)}_{key}");

            // collapser policy used to prevent concurrent calls retrieving the same data twice
            T result = await _collapserPolicy.WrapAsync(cachePolicy).ExecuteAsync(_ => dataGetter.Invoke(), context);

            return result;
        }

        private TimeSpan GetLengthOfTimeUntilNextHour()
        {
            DateTimeOffset timeNow = _mockableDateTime.UtcNow;
            DateTimeOffset nowPlusOneMinute = timeNow.AddHours(1);
            DateTimeOffset theNextMinuteWithoutSeconds = new DateTime(nowPlusOneMinute.Year, nowPlusOneMinute.Month, nowPlusOneMinute.Day, nowPlusOneMinute.Hour, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpanUntilNextMinute = theNextMinuteWithoutSeconds - timeNow;
            return timeSpanUntilNextMinute;
        }

        private TimeSpan GetLengthOfTimeUntilNextMinute()
        {
            DateTimeOffset timeNow = _mockableDateTime.UtcNow;
            DateTimeOffset nowPlusOneMinute = timeNow.AddMinutes(1);
            DateTimeOffset theNextMinuteWithoutSeconds = new DateTime(nowPlusOneMinute.Year, nowPlusOneMinute.Month, nowPlusOneMinute.Day, nowPlusOneMinute.Hour, nowPlusOneMinute.Minute, 0, DateTimeKind.Utc);
            TimeSpan timeSpanUntilNextMinute = theNextMinuteWithoutSeconds - timeNow;
            return timeSpanUntilNextMinute;
        }
    }
}
