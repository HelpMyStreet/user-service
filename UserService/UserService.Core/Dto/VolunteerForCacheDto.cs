using UserService.Core.Domains.Entities;

namespace UserService.Core.Dto
{
    public class VolunteerForCacheDto
    {
        public int UserId { get; set; }
        public string Postcode { get; set; }
        public double SupportRadiusMiles { get; set; }
        public IsVerifiedType IsVerifiedType { get; set; }
        public VolunteerType VolunteerType { get; set; }
    }
}
