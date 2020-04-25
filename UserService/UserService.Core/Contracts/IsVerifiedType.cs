using System;

namespace UserService.Core.Contracts
{
    [Flags]
    public enum IsVerifiedType : byte
    {
        IsVerified = 1,
        IsNotVerified = 2
    }
}
