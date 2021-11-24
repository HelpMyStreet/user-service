using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Contracts
{
    public class UserHistory
    {
        public string FirebaseUserId { get; set; }        
        public DateTime? CreationTimestamp { get; set; }
        public DateTime? LastSignInTimestamp { get; set; }        
    }
}
