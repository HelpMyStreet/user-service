using System.Collections.Generic;
using System.Linq;
using UserService.Core.Dto;

namespace UserService.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> WhereWithinBoundary<T>(this IEnumerable<T> source, double sWLatitude, double sWLongitude, double nELatitude, double nELongitude) where T : ILatitudeLongitude
        {
            IEnumerable<T> result = source.Where(pt =>
                pt.Latitude >= sWLatitude &&
                pt.Latitude <= nELatitude &&
                pt.Longitude >= sWLongitude &&
                pt.Longitude <= nELongitude);

            return result;
        }
    }
}
