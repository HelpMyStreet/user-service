using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Contracts
{
    public class UserLoginHistory
    {
        public int UserId { get; set; }
        public string Postcode { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? DateLastLogin { get; set; }     
    }
}
