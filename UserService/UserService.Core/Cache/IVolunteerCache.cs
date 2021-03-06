﻿using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Dto;

namespace UserService.Core
{
    public interface IVolunteerCache
    {
        Task<IEnumerable<CachedVolunteerDto>> GetCachedVolunteersAsync(VolunteerType volunteerType, CancellationToken cancellationToken);
    }
}