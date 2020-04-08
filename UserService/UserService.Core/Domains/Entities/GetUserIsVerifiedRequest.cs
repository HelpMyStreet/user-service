using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetUserIsVerifiedRequest : IRequest<GetUserIsVerifiedResponse>
    {
        public int UserID { get; set; }
    }
}
