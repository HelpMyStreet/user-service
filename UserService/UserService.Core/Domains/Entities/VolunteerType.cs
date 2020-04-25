using System;

namespace UserService.Core.Domains.Entities
{
    [Flags]
    public enum VolunteerType : byte
    {
        Helper = 1,
        StreetChampion = 2
    }
}
