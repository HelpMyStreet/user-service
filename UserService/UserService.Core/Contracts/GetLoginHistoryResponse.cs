using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Contracts
{
    public class GetLoginHistoryResponse
    {
        public List<UserHistory> History { get; set; }
    }
}
