using HelpMyStreet.Utils.CoordinatedResetCache;
using Microsoft.EntityFrameworkCore.Internal;
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
        private Mock<ICoordinatedResetCache> _coordinatedResetCache;

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

            _coordinatedResetCache = new Mock<ICoordinatedResetCache>();
            _coordinatedResetCache.Setup(x => x.GetCachedDataAsync(It.IsAny<Func<Task<IEnumerable<CachedVolunteerDto>>>>(), It.Is<string>(y => y == "AllCachedVolunteerDtos"), It.Is<CoordinatedResetCacheTime>(y => y == CoordinatedResetCacheTime.OnHour))).ReturnsAsync(_cachedVolunteerDtos);
        }

        [Test]
        public async Task GetAll()
        {
            VolunteerCache volunteerCache = new VolunteerCache(_coordinatedResetCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_coordinatedResetCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_coordinatedResetCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_coordinatedResetCache.Object, _volunteersForCacheGetter.Object);

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
            VolunteerCache volunteerCache = new VolunteerCache(_coordinatedResetCache.Object, _volunteersForCacheGetter.Object);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = IsVerifiedType.IsNotVerified;
            IEnumerable<CachedVolunteerDto> result = await volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, CancellationToken.None);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.UserId == 2));
            Assert.IsTrue(result.Any(x => x.UserId == 4));
        }

    }
}
