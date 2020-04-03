using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
    public class PersonalDetails
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Locality { get; set; }
        public string Postcode { get; set; }
        public string EmailAddress { get; set; }
        public string MobilePhone { get; set; }
        public string OtherPhone { get; set; }

        public bool? UnderlyingMedicalCondition { get; set; }

        public virtual User User { get; set; }
    }
}
