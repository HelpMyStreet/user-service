namespace UserService.Core.Utils
{
    public interface IDistanceCalculator
    {
        double GetDistanceInMetres(double longitude, double latitude, double otherLongitude, double otherLatitude);

        double GetDistanceInMiles(double longitude, double latitude, double otherLongitude, double otherLatitude);
    }
}