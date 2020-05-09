using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MediatR;

namespace UserService.Core.Domains.Entities
{
    public class GetNumberOfVolunteersInBoundaryRequest : IRequest<GetNumberOfVolunteersInBoundaryResponse>
    {
        [Required]
        [Range(-90, 90)]
        public double SwLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double SwLongitude { get; set; }

        [Required]
        [Range(-90, 90)]
        public double NeLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double NeLongitude { get; set; }
    }
}
