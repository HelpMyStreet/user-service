using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Contracts
{
    public class UserHistory
    {
        public string FirebaseUserId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? CreationTimestamp { get; set; }
        public DateTime? LastSignInTimestamp { get; set; }
        public string ErrorMessage { get; set; }

        public double? DaysSinceLastLogin
        {
            get
            {
                if(LastSignInTimestamp.HasValue)
                {
                    return Math.Floor((DateTime.Now - LastSignInTimestamp.Value).TotalDays);
                }
                else
                {
                    return null;
                }
                
            }
        }
    }
}
