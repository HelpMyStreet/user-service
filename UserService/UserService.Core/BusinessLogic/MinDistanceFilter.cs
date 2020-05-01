using System.Collections.Generic;
using UserService.Core.Dto;
using UserService.Core.Utils;

namespace UserService.Core.BusinessLogic
{
    public class MinDistanceFilter : IMinDistanceFilter
    {
        private readonly IDistanceCalculator _distanceCalculator;

        public MinDistanceFilter(IDistanceCalculator distanceCalculator)
        {
            _distanceCalculator = distanceCalculator;
        }

        public IEnumerable<T> FilterByMinDistance<T>(IEnumerable<T> latLngItems, int minDistanceBetweenInMetres) where T : ILatitudeLongitude
        {
            bool hasFirstCoordBeenAdded = false;
            List<T> latLngItemsNotNearToOthers = new List<T>();
            foreach (T cachedVolunteerDto in latLngItems)
            {
                if (!hasFirstCoordBeenAdded)
                {
                    hasFirstCoordBeenAdded = true;
                    latLngItemsNotNearToOthers.Add(cachedVolunteerDto);
                }
                else
                {
                    bool isTooNearToCoords = false;
                    foreach (T cachedVolunteerDtoWithNotNearToOthers in latLngItemsNotNearToOthers)
                    {
                        double distanceBetweenOtherCoords = _distanceCalculator.GetDistanceInMetres(cachedVolunteerDtoWithNotNearToOthers.Latitude, cachedVolunteerDtoWithNotNearToOthers.Longitude, cachedVolunteerDto.Latitude, cachedVolunteerDto.Longitude);
                        if (distanceBetweenOtherCoords < minDistanceBetweenInMetres)
                        {
                            isTooNearToCoords = true;
                            break;
                        }
                    }

                    if (!isTooNearToCoords)
                    {
                        latLngItemsNotNearToOthers.Add(cachedVolunteerDto);
                    }
                }
            }

            return latLngItemsNotNearToOthers;
        }
    }
}
