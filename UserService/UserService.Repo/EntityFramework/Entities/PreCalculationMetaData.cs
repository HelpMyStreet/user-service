using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Repo.EntityFramework.Entities
{
   public class PrecalculationMetaData
    {
        public string TableName { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
