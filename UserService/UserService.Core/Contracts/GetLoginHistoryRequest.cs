using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Contracts
{
    public class GetLoginHistoryRequest : IRequest<GetLoginHistoryResponse>
    {
    }
}
