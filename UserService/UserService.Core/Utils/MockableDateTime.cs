using System;
using Microsoft.Extensions.Internal;

namespace UserService.Core.Utils
{
    public class MockableDateTime : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTime.UtcNow;
    }
}
