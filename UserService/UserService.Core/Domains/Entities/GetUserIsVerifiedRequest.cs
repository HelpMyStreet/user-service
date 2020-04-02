using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetUserIsVerifiedRequest : IRequest<GetUserIsVerifiedResponse>
    {
        public string ID { get; set; }
    }
}
