using Polly.Caching.Memory;

namespace UserService.Core
{
    public class PollyMemoryCacheProvider : IPollyMemoryCacheProvider
    {
        private static Polly.Caching.Memory.MemoryCacheProvider _memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));

        public MemoryCacheProvider MemoryCacheProvider => _memoryCacheProvider;
    }
}
