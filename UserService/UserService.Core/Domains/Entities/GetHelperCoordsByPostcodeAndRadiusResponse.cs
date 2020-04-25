using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UserService.Core.Domains.Entities
{
    [DataContract(Name = "getHelperCoordsByPostcodeAndRadiusResponse")]
    public class GetHelperCoordsByPostcodeAndRadiusResponse
    {
        [DataMember(Name = "volunteerCoordinates")]
        public IReadOnlyList<VolunteerCoordinate> Coordinates { get; set; }
    }
}
