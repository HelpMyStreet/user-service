using HelpMyStreet.Utils.Enums;

namespace UserService.Core.Dto
{
    public class VolunteerForCacheDto
    {
        public int UserId { get; set; }
        public string Postcode { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double SupportRadiusMiles { get; set; }
        public IsVerifiedType IsVerifiedType { get; set; }
        public VolunteerType VolunteerType { get; set; }
    }
}
