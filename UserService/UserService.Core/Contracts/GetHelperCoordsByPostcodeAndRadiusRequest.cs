using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserService.Core.Contracts
{
    public class GetHelperCoordsByPostcodeAndRadiusRequest : IRequest<GetHelperCoordsByPostcodeAndRadiusResponse>
    {
        [Required]
        public string Postcode { get; set; }

        [Required]
        public int RadiusInMetres { get; set; }

        [Required]
        public IEnumerable<VolunteerType> UserTypes { get; set; }

        [Required]
        public bool GetVerifiedUsersOnly { get; set; }
    }
}
