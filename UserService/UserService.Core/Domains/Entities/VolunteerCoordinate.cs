using System.Runtime.Serialization;

namespace UserService.Core.Domains.Entities
{
    [DataContract(Name = "volunteerCoordinate")]
    public class VolunteerCoordinate 
    {
        [DataMember(Name = "pc")]
        public string Postcode { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        // fields are temporarily nullable until the grid aggregation functionality is implemented
        [DataMember(Name = "sc")]
        public double? NumberOfStreetChampions { get; set; }

        [DataMember(Name = "h")]
        public double? NumberOfHelpers { get; set; }

    }
}
