using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
    public class SupportActivity
    {
        public int UserId { get; set; }
        public byte ActivityId { get; set; }

        public virtual User User { get; set; }
    }
}
