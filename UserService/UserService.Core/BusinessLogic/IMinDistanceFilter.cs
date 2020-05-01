using System.Collections.Generic;
using UserService.Core.Dto;

namespace UserService.Core.BusinessLogic
{
    public interface IMinDistanceFilter
    {
        IEnumerable<T> FilterByMinDistance<T>(IEnumerable<T> latLngItems, int minDistanceBetweenInMetres) where T : ILatitudeLongitude;
    }
}