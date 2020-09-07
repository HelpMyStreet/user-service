using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Utils;

namespace UserService.Core.Services
{
    public class HelperService : IHelperService
    {
        private readonly IVolunteerCache _volunteerCache;
        private readonly IDistanceCalculator _distanceCalculator;        
        private readonly IRepository _repository;
        public HelperService(IVolunteerCache volunteerCache, IDistanceCalculator distanceCalculator, IRepository repository)
        {
            _volunteerCache = volunteerCache;
            _distanceCalculator = distanceCalculator;
            _repository = repository;
        }


        public async Task<IEnumerable<HelperWithinRadiusDTO>> GetHelpersWithinRadius(string postcode, IsVerifiedType verifiedType, CancellationToken token)
        {
            var helpers = new List<HelperWithinRadiusDTO>();
            LatitudeAndLongitudeDTO comparePostcode = _repository.GetLatitudeAndLongitude(postcode);

            VolunteerType volunteerType = VolunteerType.Helper | VolunteerType.StreetChampion;
            IsVerifiedType isVerifiedType = verifiedType;
            Task<IEnumerable<CachedVolunteerDto>> cachedVolunteerDtosTask = _volunteerCache.GetCachedVolunteersAsync(volunteerType, isVerifiedType, token);

            await Task.WhenAll(cachedVolunteerDtosTask);
            IEnumerable<CachedVolunteerDto> cachedVolunteerDtos = await cachedVolunteerDtosTask;
       
            Dictionary<int, double> idsOfHelpersWithinRadius = new Dictionary<int, double>();

            foreach (CachedVolunteerDto cachedVolunteerDto in cachedVolunteerDtos)
            {
                double distance = _distanceCalculator.GetDistanceInMiles(comparePostcode.Latitude, comparePostcode.Longitude, cachedVolunteerDto.Latitude, cachedVolunteerDto.Longitude);

                bool isWithinSupportRadius = distance <= cachedVolunteerDto.SupportRadiusMiles;

                if (isWithinSupportRadius)
                {
                    idsOfHelpersWithinRadius.Add(cachedVolunteerDto.UserId, distance);
                }
            }
            var users = await _repository.GetVolunteersByIdsAsync(idsOfHelpersWithinRadius.Keys);



            if (users.Any())
            {
                if (isVerifiedType != IsVerifiedType.All)
                {
                    users = users.Where(x => x.IsVerified == (verifiedType == IsVerifiedType.IsVerified) ? true : false);
                }
                helpers = users.Select(x => new HelperWithinRadiusDTO { User = x, Distance = idsOfHelpersWithinRadius[x.ID] }).ToList();
            }
            return helpers;
        }
    }
}
