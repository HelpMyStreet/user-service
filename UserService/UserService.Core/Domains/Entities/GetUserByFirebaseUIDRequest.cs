using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetUserByFirebaseUIDRequest : IRequest<GetUserByFirebaseUIDResponse>
    {
        public string FirebaseUID { get; set; }
    }
}
