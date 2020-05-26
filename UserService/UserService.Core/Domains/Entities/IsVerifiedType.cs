using System;

namespace UserService.Core.Domains.Entities
{
    [Flags]
    public enum IsVerifiedType : byte
    {
        IsVerified = 1,
        IsNotVerified = 2,
        All = 3
    }
}
