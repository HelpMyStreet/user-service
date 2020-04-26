using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Moq;
using NUnit.Framework;
using Polly.Caching.Memory;
using UserService.Core;
using UserService.Core.Cache;

namespace UserService.UnitTests
{
    public class CoordinatedResetCacheTests
    {
        private Mock<ISystemClock> _mockableDateTime;

        private Mock<ITestDataGetter> _dataGetter1;
        private Mock<ITestDataGetter> _dataGetter2;

        private Func<Task<string>> _dataGetterDelegate1;
        private Func<Task<string>> _dataGetterDelegate2;

        [SetUp]
        public void SetUp()
        {
            _dataGetter1 = new Mock<ITestDataGetter>();
            _dataGetter1.Setup(x => x.GetDataAsync()).ReturnsAsync("hello");
            _dataGetterDelegate1 = async () =>
            {
                await Task.Delay(50);
                return await _dataGetter1.Object.GetDataAsync();
            };

            _dataGetter2 = new Mock<ITestDataGetter>();
            _dataGetter2.Setup(x => x.GetDataAsync()).ReturnsAsync("goodbye");
            _dataGetterDelegate2 = async () =>
            {
                await Task.Delay(50);
                return await _dataGetter2.Object.GetDataAsync();
            };

            _mockableDateTime = new Mock<ISystemClock>();
        }


        [Test]
        public async Task CacheExpiresOnTheHour()
        {
            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00, 00, DateTimeKind.Utc));

            Mock<IPollyMemoryCacheProvider> pollyMemoryCacheProvider = new Mock<IPollyMemoryCacheProvider>();

            MemoryCacheProvider memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()
            {
                Clock = _mockableDateTime.Object
            }));
            pollyMemoryCacheProvider.SetupGet(x => x.MemoryCacheProvider).Returns(memoryCacheProvider);


            CoordinatedResetCache coordinatedResetCache = new CoordinatedResetCache(pollyMemoryCacheProvider.Object, _mockableDateTime.Object);

            string result1 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnHour);
            string result2 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnHour);

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 16, 00, 00, 00, DateTimeKind.Utc));

            string result3 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnHour);
            string result4 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnHour);


            Assert.AreEqual("hello", result1);
            Assert.AreEqual("hello", result2);
            Assert.AreEqual("hello", result3);
            Assert.AreEqual("hello", result4);

            _dataGetter1.Verify(x => x.GetDataAsync(), Times.Exactly(2));

        }

        [Test]
        public async Task CacheExpiresOnTheMinute()
        {
            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00, 00, DateTimeKind.Utc));

            Mock<IPollyMemoryCacheProvider> pollyMemoryCacheProvider = new Mock<IPollyMemoryCacheProvider>();

            MemoryCacheProvider memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()
            {
                Clock = _mockableDateTime.Object
            }));
            pollyMemoryCacheProvider.SetupGet(x => x.MemoryCacheProvider).Returns(memoryCacheProvider);


            CoordinatedResetCache coordinatedResetCache = new CoordinatedResetCache(pollyMemoryCacheProvider.Object, _mockableDateTime.Object);

            string result1 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnMinute);
            string result2 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnMinute);

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 46, 00, 00, DateTimeKind.Utc));

            string result3 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnMinute);
            string result4 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnMinute);


            Assert.AreEqual("hello", result1);
            Assert.AreEqual("hello", result2);
            Assert.AreEqual("hello", result3);
            Assert.AreEqual("hello", result4);

            _dataGetter1.Verify(x => x.GetDataAsync(), Times.Exactly(2));

        }

        [Test]
        public async Task DataIsOnlyRetrievedOnceDuringConcurrentCalls()
        {
            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00, 00, DateTimeKind.Utc));

            Mock<IPollyMemoryCacheProvider> pollyMemoryCacheProvider = new Mock<IPollyMemoryCacheProvider>();

            MemoryCacheProvider memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()
            {
                Clock = _mockableDateTime.Object
            }));
            pollyMemoryCacheProvider.SetupGet(x => x.MemoryCacheProvider).Returns(memoryCacheProvider);

            CoordinatedResetCache coordinatedResetCache = new CoordinatedResetCache(pollyMemoryCacheProvider.Object, _mockableDateTime.Object);

            ConcurrentBag<Task<string>> results = new ConcurrentBag<Task<string>>();
            Parallel.For(0, 50, i =>
            {
                Task<string> result = coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnMinute);
                results.Add(result);
            });

            await Task.WhenAll(results);

            foreach (Task<string> result in results)
            {
                Assert.AreEqual("hello", await result);
            }

            _dataGetter1.Verify(x => x.GetDataAsync(), Times.Exactly(1));

        }


        [Test]
        public async Task TwoSourcesOfData()
        {
            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00, 00, DateTimeKind.Utc));

            Mock<IPollyMemoryCacheProvider> pollyMemoryCacheProvider = new Mock<IPollyMemoryCacheProvider>();

            MemoryCacheProvider memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()
            {
                Clock = _mockableDateTime.Object
            }));
            pollyMemoryCacheProvider.SetupGet(x => x.MemoryCacheProvider).Returns(memoryCacheProvider);


            CoordinatedResetCache coordinatedResetCache = new CoordinatedResetCache(pollyMemoryCacheProvider.Object, _mockableDateTime.Object);

            string result1 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key1", CoordinatedResetCacheTime.OnHour);
            string result2 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate2, "key2", CoordinatedResetCacheTime.OnHour);

            string result3 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate1, "key1", CoordinatedResetCacheTime.OnHour);
            string result4 = await coordinatedResetCache.GetCachedDataAsync<string>(_dataGetterDelegate2, "key2", CoordinatedResetCacheTime.OnHour);

            Assert.AreEqual("hello", result1);
            Assert.AreEqual("goodbye", result2);
            Assert.AreEqual("hello", result3);
            Assert.AreEqual("goodbye", result4);

            _dataGetter1.Verify(x => x.GetDataAsync(), Times.Exactly(1));
            _dataGetter2.Verify(x => x.GetDataAsync(), Times.Exactly(1));

        }

        [Test]
        public async Task TwoInstancesShareBackingMemory()
        {
            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00, 00, DateTimeKind.Utc));

            Mock<IPollyMemoryCacheProvider> pollyMemoryCacheProvider = new Mock<IPollyMemoryCacheProvider>();

            MemoryCacheProvider memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()
            {
                Clock = _mockableDateTime.Object
            }));
            pollyMemoryCacheProvider.SetupGet(x => x.MemoryCacheProvider).Returns(memoryCacheProvider);


            CoordinatedResetCache coordinatedResetCache1 = new CoordinatedResetCache(pollyMemoryCacheProvider.Object, _mockableDateTime.Object);
            CoordinatedResetCache coordinatedResetCache2 = new CoordinatedResetCache(pollyMemoryCacheProvider.Object, _mockableDateTime.Object);

            string result1 = await coordinatedResetCache1.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnHour);
            string result2 = await coordinatedResetCache2.GetCachedDataAsync<string>(_dataGetterDelegate1, "key", CoordinatedResetCacheTime.OnHour);


            Assert.AreEqual("hello", result1);
            Assert.AreEqual("hello", result2);

            _dataGetter1.Verify(x => x.GetDataAsync(), Times.Exactly(1));

        }


    }
}
