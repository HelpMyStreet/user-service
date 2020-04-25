using System;

namespace UserService.Core.Utils
{
    public class MockableDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now =>DateTime.Now;
    }
}
