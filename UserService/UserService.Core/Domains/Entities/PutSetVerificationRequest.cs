using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class PutSetVerificationRequest : IRequest<PutSetVerificationResponse>
    {
        public int UserID { get; set; }
        public bool IsVerified { get; set; }
        public string ServiceName { get; set; }
    }
}
