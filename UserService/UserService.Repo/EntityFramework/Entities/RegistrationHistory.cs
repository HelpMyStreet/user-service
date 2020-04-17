using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
    public class RegistrationHistory
    {
        public int UserId { get; set; }
        public byte RegistrationStep { get; set; }
        public DateTime DateCompleted { get; set; }

        public virtual User User { get; set; }
    }
}
