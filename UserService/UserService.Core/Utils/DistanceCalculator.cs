using System;
using HelpMyStreet.Utils.Utils;

namespace UserService.Core.Utils
{
    public class DistanceCalculator : IDistanceCalculator
    {

        public double GetDistanceInMetres(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            // Stolen from Microsoft's GeoCoordinate class (not yet available for .Net Core)
            double d1 = latitude * (Math.PI / 180.0);
            double num1 = longitude * (Math.PI / 180.0);
            double d2 = otherLatitude * (Math.PI / 180.0);
            double num2 = otherLongitude * (Math.PI / 180.0) - num1;
            double d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        public double GetDistanceInMiles(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var distanceInMetres = GetDistanceInMetres(longitude, latitude, otherLongitude, otherLatitude);
            var distanceInMiles = DistanceConverter2.MetresToMiles(distanceInMetres);
            return distanceInMiles;
        }

    }

    public static class DistanceConverter2
    {
        public static double MetresToMiles(double metres)
        {
            return metres / 1609.344d;
        }
    }
}
