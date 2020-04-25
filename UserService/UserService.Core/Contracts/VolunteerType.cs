using System;

namespace UserService.Core.Contracts
{
    [Flags]
    public enum VolunteerType : byte
    {
        Helper = 1 << 0, // 1
        StreetChampion = 1 << 1 // 2
    }
}
