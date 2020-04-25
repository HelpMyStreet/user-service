using System;

namespace UserService.Core.Utils
{
    public interface IDateTime
    {
        DateTime UtcNow { get; }

        DateTime Now { get; }
    }
}
