using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Contracts
{
    public class UserLoginHistory
    {
        public int UserId { get; set; }
        public string Postcode { get; set; }
        public DateTime? DateLastLogin { get; set; }     
    }
}
