using HelpMyStreet.Cache;
using HelpMyStreet.Utils.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Cache;
using UserService.Core.Dto;

namespace UserService.AzureFunction
{
    public class TimedCacheRefresher
    {
        private readonly IMemDistCache<IEnumerable<CachedVolunteerDto>> _memDistCache;
        private readonly IVolunteersForCacheGetter _volunteersForCacheGetter;
        private readonly ILoggerWrapper<TimedCacheRefresher> _logger;

        public TimedCacheRefresher(IMemDistCache<IEnumerable<CachedVolunteerDto>> memDistCache, IVolunteersForCacheGetter volunteersForCacheGetter, ILoggerWrapper<TimedCacheRefresher> logger)
        {
            _memDistCache = memDistCache;
            _volunteersForCacheGetter = volunteersForCacheGetter;
            _logger = logger;
        }

        [FunctionName("TimedCacheRefresher")]
        public async Task Run([TimerTrigger("%TimedCacheRefresherCronExpression%")] TimerInfo timerInfo, CancellationToken cancellationToken, ILogger log)
        {
            _logger.LogInformation($"TimedCacheRefresher CRON trigger executed at : {DateTimeOffset.Now}");

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await _memDistCache.RefreshDataAsync(async (token) => await _volunteersForCacheGetter.GetAllVolunteersAsync(token), CacheKey.AllCachedVolunteerDtos.ToString(), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error running TimedCacheRefresher", ex);
            }

            stopwatch.Stop();

            _logger.LogInformation($"TimedCacheRefresher CRON trigger took: {stopwatch.Elapsed:%m' min '%s' sec'}");
        }
    }
}
