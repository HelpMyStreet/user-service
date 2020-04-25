using System;

namespace UserService.Core.Contracts
{
    [Flags]
    public enum IsVerifiedType : byte
    {
        IsVerified = 1 << 0, // 1
        IsNotVerified = 1 << 1 // 2
    }
}
