using UserService.Core.Domains.Entities;

namespace UserService.Repo.EntityFramework.Entities
{
   public class PrecalculatedVolunteer
    {
        public int UserId { get; set; }
        public string Postcode { get; set; }
        public double SupportRadiusMiles { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public VolunteerType VolunteerType { get; set; }
        public IsVerifiedType IsVerifiedType { get; set; }
    }
}
