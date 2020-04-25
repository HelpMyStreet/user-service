using Polly.Caching.Memory;

namespace UserService.Core
{
    public interface IPollyMemoryCacheProvider
    {
        MemoryCacheProvider MemoryCacheProvider { get; }
    }
}