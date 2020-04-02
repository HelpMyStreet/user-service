using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetVolunteerCountByPostcodeRequest : IRequest<GetVolunteerCountByPostcodeResponse>
    {
        public string PostCode { get; set; }
    }
}
