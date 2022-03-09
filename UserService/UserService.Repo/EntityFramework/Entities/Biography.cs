using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
    public class Biography
    {
        public int UserId { get; set; }
        public string Details { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual User User { get; set; }
    }
}
