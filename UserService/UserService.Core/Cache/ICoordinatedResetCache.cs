using System;
using System.Threading.Tasks;
using UserService.Core.Cache;

namespace UserService.Core
{
    public interface ICoordinatedResetCache
    {
        /// <summary>
        ///  Get data from cache. Cache expires on the hour or minute so all servers are kept in sync. IPollyMemoryCacheProvider and ISystemClock must be registered in DI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that return data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="resetCacheTime">When cache should reset</param>
        /// <returns></returns>
        Task<T> GetCachedDataAsync<T>(Func<Task<T>> dataGetter, string key, CoordinatedResetCacheTime resetCacheTime);
    }
}