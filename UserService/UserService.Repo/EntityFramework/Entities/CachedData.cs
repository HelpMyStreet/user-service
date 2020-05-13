using System;
using UserService.Core.Domains.Entities;

namespace UserService.Repo.EntityFramework.Entities
{
    public class CachedData
    {
        public string Key { get; set; }
        public DateTime LastUpdated { get; set; }
        public byte[] Data { get; set; }
    }
}
