using System;

namespace UserService.Core.Utils
{
    public class DistanceCalculator : IDistanceCalculator
    {

        public double GetDistanceInMetres(double latitude, double longitude, double otherLatitude, double otherLongitude)
        {
            // Stolen from Microsoft's GeoCoordinate class (not yet available for .Net Core)
            double d1 = latitude * (Math.PI / 180.0);
            double num1 = longitude * (Math.PI / 180.0);
            double d2 = otherLatitude * (Math.PI / 180.0);
            double num2 = otherLongitude * (Math.PI / 180.0) - num1;
            double d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        public double GetDistanceInMiles(double latitude, double longitude, double otherLatitude, double otherLongitude)
        {
            var distanceInMetres = GetDistanceInMetres(latitude, longitude, otherLatitude, otherLongitude);
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
