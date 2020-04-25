using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace UserService.Core.Domains.Entities
{
    [DataContract(Name = "getPostcodeCoordinatesResponse")]
    public class GetPostcodeCoordinatesResponse
    {
        [Required]
        [DataMember(Name = "postcodeCoordinates")]
        public IReadOnlyList<PostcodeCoordinate> PostcodeCoordinates { get; set; }
    }
}
