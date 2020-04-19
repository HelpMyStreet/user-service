using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
    public class User
    {
        public User()
        {
            ChampionPostcode = new HashSet<ChampionPostcode>();
            SupportActivity = new HashSet<SupportActivity>();
            SupportPostcode = new HashSet<SupportPostcode>();
            RegistrationHistory = new HashSet<RegistrationHistory>();
        }

        public int Id { get; set; }
        public string FirebaseUid { get; set; }
        public string PostalCode { get; set; }
        public bool? EmailSharingConsent { get; set; }
        public bool? MobileSharingConsent { get; set; }
        public bool? OtherPhoneSharingConsent { get; set; }
        public bool? HmscontactConsent { get; set; }
        public bool? IsVolunteer { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? DateCreated { get; set; }
        public double? SupportRadiusMiles { get; set; }
        public bool? SupportVolunteersByPhone { get; set; }

        public bool? StreetChampionRoleUnderstood { get; set; }

        public virtual PersonalDetails PersonalDetails { get; set; }
        public virtual ICollection<ChampionPostcode> ChampionPostcode { get; set; }
        public virtual ICollection<SupportActivity> SupportActivity { get; set; }
        public virtual ICollection<SupportPostcode> SupportPostcode { get; set; }
        public virtual ICollection<RegistrationHistory> RegistrationHistory { get; set; }
    }
}
