using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AddressService.Core.Contracts
{
    public class GetPostcodeCoordinatesRequest : IRequest<GetPostcodeCoordinatesResponse>
    {
        [Required]
        public IEnumerable<string> Postcodes { get; set; }
    }
}
