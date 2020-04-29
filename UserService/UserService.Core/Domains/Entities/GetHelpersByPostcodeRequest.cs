using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetHelpersByPostcodeRequest : IRequest<GetHelpersByPostcodeResponse>
    {
        public string Postcode { get; set; }
    }
}
