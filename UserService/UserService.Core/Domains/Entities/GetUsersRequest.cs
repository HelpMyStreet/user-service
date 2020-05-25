using HelpMyStreet.Contracts.UserService.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetUsersRequest : IRequest<GetUsersResponse>
    {
    }
}
