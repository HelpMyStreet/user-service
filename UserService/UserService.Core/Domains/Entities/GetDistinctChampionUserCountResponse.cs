using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetDistinctChampionUserCountResponse
    {
        public int Count { get; set; }
    }
}
