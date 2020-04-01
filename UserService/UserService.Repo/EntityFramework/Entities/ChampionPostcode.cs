using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
    public class ChampionPostcode
    {
        public int UserId { get; set; }
        public string PostalCode { get; set; }
        public virtual User User { get; set; }
    }
}
