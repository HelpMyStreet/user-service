using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "postcodeCoordinate")]
    public class PostcodeCoordinate
    {
        [DataMember(Name = "pc")]
        public string Postcode { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }
}
