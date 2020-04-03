using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetUserByIDRequest : IRequest<GetUserByIDResponse>
    {
        public int ID { get; set; }
    }
}
