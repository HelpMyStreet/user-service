using System;

namespace UserService.Core.Contracts
{
    [Flags]
    public enum VolunteerType : byte
    {
        Helper = 1,
        StreetChampion = 2
    }
}
