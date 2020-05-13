using System;

namespace UserService.Core.Dto
{
    public class CachedDataDto
    {
        public string Key { get; set; }
        public DateTime LastUpdated { get; set; }
        public byte[] Data { get; set; }
    }
}
