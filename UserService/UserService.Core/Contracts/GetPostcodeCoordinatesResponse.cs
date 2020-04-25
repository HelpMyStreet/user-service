using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace AddressService.Core.Contracts
{
    [DataContract(Name = "getPostcodeCoordinatesResponse")]
    public class GetPostcodeCoordinatesResponse
    {
        [Required]
        [DataMember(Name = "postcodeCoordinates")]
        public IEnumerable<PostcodeCoordinate> PostcodeCoordinates { get; set; }
    }
}
