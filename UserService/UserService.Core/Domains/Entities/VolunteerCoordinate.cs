using System.Runtime.Serialization;

namespace UserService.Core.Domains.Entities
{
    [DataContract(Name = "volunteerCoordinate")]
    public class VolunteerCoordinate
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "type")]
        public VolunteerType VolunteerType { get; set; }

        [DataMember(Name = "verif")]
        public bool IsVerified { get; set; }
    }
}
