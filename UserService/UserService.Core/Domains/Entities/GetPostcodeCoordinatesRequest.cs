using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace UserService.Core.Domains.Entities
{
    public class GetPostcodeCoordinatesRequest : IRequest<GetPostcodeCoordinatesResponse>
    {
        [Required]
        public IEnumerable<string> Postcodes { get; set; }
    }
}
