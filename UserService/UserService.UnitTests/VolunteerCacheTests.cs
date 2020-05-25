using HelpMyStreet.Cache;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core;
using UserService.Core.Cache;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;

namespace UserService.UnitTests
{
    public class VolunteerCacheTests
    {

        private Mock<IVolunteersForCacheGetter> _volunteersForCacheGetter;
        private Mock<IMemDistCache<IEnumerable<CachedVolunteerDto>>> _memDistCache;

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
            _volunteersForCacheGetter.SetupAllProperties();

            _memDistCache = new Mock<IMemDistCache<IEnumerable<CachedVolunteerDto>>>();
            _memDistCache.Setup(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<IEnumerable<CachedVolunteerDto>>>>(), It.Is<string>(y => y == "AllCachedVolunteerDtos"), It.Is<RefreshBehaviour>(y => y == RefreshBehaviour.DontRefreshData), It.IsAny<CancellationToken>())).ReturnsAsync(_cachedVolunteerDtos);
        }

        [Test]
        public async Task GetAll()
        {
            VolunteerCache volunteerCache = new VolunteerCache(_memDistCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_memDistCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_memDistCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_memDistCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_memDistCache.Object, _volunteersForCacheGetter.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 2));
            Assert.IsTrue(result.Any(x => x.UserId == 4));
        }

    }
}
