using System.Runtime.Serialization;

namespace UserService.Core.Contracts
{
    [DataContract(Name = "volunteerCoordinate")]
    public class VolunteerCoordinate
    {
        [DataMember(Name = "latitude")]
        public decimal Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public decimal Longitude { get; set; }

        [DataMember(Name = "volunteerType")]
        public VolunteerType VolunteerType { get; set; }
    }
}
