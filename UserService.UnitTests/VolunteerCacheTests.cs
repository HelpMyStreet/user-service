using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Internal;
using Moq;
using NUnit.Framework;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.UnitTests
{
    public class VolunteerCacheTests
    {

        private Mock<IVolunteersForCacheGetter> _volunteersForCacheGetter;
        private Mock<ISystemClock> _mockableDateTime;

        private IEnumerable<CachedVolunteerDto> _cachedVolunteerDtos;

        [SetUp]
        public void SetUp()
        {

            _cachedVolunteerDtos = new List<CachedVolunteerDto>()
            {
                new CachedVolunteerDto()
                {
                    UserId = 1,
                    Postcode = "NG1 1AA",
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsVerified
                },
                new CachedVolunteerDto()
                {
                    UserId = 2,
                    Postcode = "NG1 1AB",
                    VolunteerType = VolunteerType.Helper,
                    IsVerifiedType = IsVerifiedType.IsNotVerified
                },
                new CachedVolunteerDto()
                {
                    UserId = 3,
                    Postcode = "NG1 1AC",
                    VolunteerType = VolunteerType.StreetChampion,
                    IsVerifiedType = IsVerifiedType.IsVerified
                },
                new CachedVolunteerDto()
                {
                    UserId = 4,
                    Postcode = "NG1 1AD",
                    VolunteerType = VolunteerType.StreetChampion,
                    IsVerifiedType = IsVerifiedType.IsNotVerified
                },
            };

            _volunteersForCacheGetter = new Mock<IVolunteersForCacheGetter>();

            _volunteersForCacheGetter.Setup(x => x.GetAllVolunteersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);

            _mockableDateTime = new Mock<ISystemClock>();
            _mockableDateTime.SetupGet(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00));


        }

        [Test]
        public async Task GetAll()
        {
            VolunteerCache volunteerCache = new VolunteerCache(new PollyMemoryCacheProvider(), _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);


            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 1));
            Assert.IsTrue(result.Any(x => x.UserId == 2));
            Assert.IsTrue(result.Any(x => x.UserId == 3));
            Assert.IsTrue(result.Any(x => x.UserId == 4));

        }

        [Test]
        public async Task GetAllStreetChampions()
        {
            VolunteerCache volunteerCache = new VolunteerCache(new PollyMemoryCacheProvider(), _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);


            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 3));
            Assert.IsTrue(result.Any(x => x.UserId == 4));
        }

        [Test]
        public async Task GetAllHelpers()
        {
            VolunteerCache volunteerCache = new VolunteerCache(new PollyMemoryCacheProvider(), _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.Helper;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);


            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 1));
            Assert.IsTrue(result.Any(x => x.UserId == 2));
        }

        [Test]
        public async Task GetAllVerfied()
        {
            VolunteerCache volunteerCache = new VolunteerCache(new PollyMemoryCacheProvider(), _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);


            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 1));
            Assert.IsTrue(result.Any(x => x.UserId == 3));
        }


        [Test]
        public async Task GetAllNonVerfied()
        {
            VolunteerCache volunteerCache = new VolunteerCache(new PollyMemoryCacheProvider(), _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);


            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 2));
            Assert.IsTrue(result.Any(x => x.UserId == 4));
        }

        [Test]
        public async Task CacheWorks()
        {

            _mockableDateTime.SetupGet(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 15, 45, 00, 00));
            Mock<IPollyMemoryCacheProvider> pollyMemoryCacheProvider = new Mock<IPollyMemoryCacheProvider>();

            MemoryCacheProvider memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()
            {
                Clock = _mockableDateTime.Object
            }));
            pollyMemoryCacheProvider.SetupGet(x => x.MemoryCacheProvider).Returns(memoryCacheProvider);


            VolunteerCache volunteerCache = new VolunteerCache(pollyMemoryCacheProvider.Object, _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result1 = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);
            IEnumerable<CachedVolunteerDto> result2 = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);

            Assert.AreEqual(4, result1.Count());
            Assert.AreEqual(4, result2.Count());

            _volunteersForCacheGetter.Verify(x => x.GetAllVolunteersAsync(It.IsAny<CancellationToken>()), Times.Once);
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


            VolunteerCache volunteerCache = new VolunteerCache(pollyMemoryCacheProvider.Object, _volunteersForCacheGetter.Object, _mockableDateTime.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsVerified | IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result1 = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);
            IEnumerable<CachedVolunteerDto> result2 = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTime(2020, 04, 24, 16, 00, 00, 00, DateTimeKind.Utc));

            IEnumerable<CachedVolunteerDto> result3 = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);
            IEnumerable<CachedVolunteerDto> result4 = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);


            Assert.AreEqual(4, result1.Count());
            Assert.AreEqual(4, result2.Count());
            Assert.AreEqual(4, result3.Count());
            Assert.AreEqual(4, result4.Count());

            _volunteersForCacheGetter.Verify(x => x.GetAllVolunteersAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));

        }
    }
}
